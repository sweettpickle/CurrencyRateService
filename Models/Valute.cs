using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CurrencyExchangeRateService.Models
{
    [Serializable]
    public class Valute
    {
        [XmlAttribute("ID")]
        public string ExternalId { get; set; }
        [XmlElement]
        public string Value { get; set; }
    }
}
