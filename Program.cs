using CurrencyExchangeRateService.DAL;
using CurrencyExchangeRateService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CurrencyExchangeRateService
{
    public class Program
    {
        private static int timeout { get; set; } = 86400000;  //24 hour = 86400000 ms

        static void Main(string[] args)
        {
            while (true)
            {
                //LoadValuteInformation(); //прогрузить информацию о валютах 

                var date = DateTime.Now.Date;
                Task.Run(() => 
                    UpdateCurrencyRate(date)
                ); 
                Thread.Sleep(timeout);

                //Console.WriteLine(get_rate("AUD", date)); // получить курс по дате и чаркоду валюты
            }
        }

        public static async Task UpdateCurrencyRate(DateTime date)
        {
            Console.WriteLine($"Start update currency rate for {date.ToString("dd.MM.yyyy")}");

            HttpClient client = new HttpClient();
            var uri = $"http://www.cbr.ru/scripts/XML_daily.asp?date_req={date.ToString("dd.MM.yyyy")}";
            //var uri = $"http://www.cbr.ru/scripts/XML_daily.asp?date_req=21.08.2019"; //for test

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                string responseString = Encoding.GetEncoding("windows-1251").GetString(bytes, 0, bytes.Length);

                XmlSerializer serializer = new XmlSerializer(typeof(ValCurs));
                ValCurs valCurs;
                using (var sr = new StringReader(responseString))
                    valCurs = (ValCurs)serializer.Deserialize(sr);

                if (valCurs.Valutes.Any())
                {
                    using (CurrencyRateContext db = new CurrencyRateContext())
                    {
                        var valutes = db.Valute.ToList();
                        foreach (var item in valCurs.Valutes)
                        {
                            var valute = valutes.Where(x => x.ExternalId == item.ExternalId).FirstOrDefault();
                            if (valute is null)
                            {
                                Console.WriteLine($"Valute with externalId {item.ExternalId} not found.");
                                continue;
                            }

                            double value;
                            if (!double.TryParse(item.Value, out value))
                            {
                                Console.WriteLine($"Cant parse value {item.Value} of valute externalId {item.ExternalId}");
                                continue;
                            }

                            var currencyRate = new CurrencyRate()
                            {
                                Valute = valute,
                                Value = value,
                                Date = date
                            };

                            var isExist = db.CurrencyRate.Where(x => x.Date == date && x.Valute.ExternalId == item.ExternalId).FirstOrDefault();
                            if (isExist != null)//если запись сущ, то обновляем
                                isExist.Value = currencyRate.Value;
                            else
                                await db.CurrencyRate.AddAsync(currencyRate);
                        }

                        await db.SaveChangesAsync();
                        Console.WriteLine($"The currency rate is loaded");
                    }
                }
            }

            Console.WriteLine($"End update currency rate for {date.ToString("dd.MM.yyyy")}");
        }

        public static async void LoadValuteInformation()
        {
            HttpClient client = new HttpClient();
            var uri = $"http://www.cbr.ru/scripts/XML_valFull.asp";

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                string responseString = Encoding.GetEncoding("windows-1251").GetString(bytes, 0, bytes.Length);

                XmlSerializer serializer = new XmlSerializer(typeof(ValuteInformation));
                ValuteInformation valuteInformation;
                using (var sr = new StringReader(responseString))
                    valuteInformation = (ValuteInformation)serializer.Deserialize(sr);

                if (valuteInformation.Items.Any())
                {
                    using (CurrencyRateContext db = new CurrencyRateContext())
                    {
                        foreach (var item in valuteInformation.Items)
                        {
                            var isExist = db.Valute.Where(x => x.ExternalId == item.ExternalId).FirstOrDefault();
                            if (isExist != null) //если валюта уже сущ, то обновляем данные
                            {
                                isExist.Name = item.Name;
                                isExist.EngName = item.EngName;
                                isExist.Nominal = item.Nominal;
                                isExist.NumCode = item.NumCode;
                                isExist.CharCode = item.CharCode;
                                Console.WriteLine($"Record with externalId {item.ExternalId} will be update");
                            }
                            else
                                await db.Valute.AddAsync(item);
                        }
                        
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        public static double? get_rate(string charCode, DateTime date)
        {
            using (CurrencyRateContext db = new CurrencyRateContext())
            {
                var res = db.CurrencyRate.Where(x => x.Valute.CharCode != null && x.Valute.CharCode.ToLower() == charCode.ToLower() 
                && x.Date == date).FirstOrDefault();

                if (res is null)
                    return null;
                else
                    return res.Value;
            }
        }
    }
}


//CREATE FUNCTION get_rate(charCode text, datte date) RETURNS real[] AS '
//  select "Value"
//	from "CurrencyRate" 
//	inner join "Valute" on "CurrencyRate"."ValuteId" = "Valute"."Id"
//	where cast("Date" as DATE) = cast(datte as DATE)
//  and lower("CharCode") = lower('AUD')
//' LANGUAGE SQL;

//SELECT get_rate('AUD', cast (now() as DATE));