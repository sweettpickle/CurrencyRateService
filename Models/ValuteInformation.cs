using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CurrencyExchangeRateService.Models
{
    [Serializable]
    [XmlRoot("Valuta")]
    public class ValuteInformation
    {
        [XmlElement("Item")]
        public List<DAL.Valute> Items { get; set; }
    }
}
