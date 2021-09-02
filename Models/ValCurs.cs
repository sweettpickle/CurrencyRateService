using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CurrencyExchangeRateService.Models
{
    [Serializable]
    [XmlRoot("ValCurs")]
    public class ValCurs
    {
        [XmlElement("Valute")]
        public List<Valute> Valutes { get; set; }
    }
}
