using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CurrencyExchangeRateService.DAL
{
    [Serializable]
    public class Valute
    {
        [Key]
        public Guid Id { get; set; }
        [XmlAttribute("ID")]
        public string ExternalId { get; set; }
        [XmlElement("ISO_Num_Code")]
        public string NumCode { get; set; }
        [XmlElement("ISO_Char_Code")]
        public string CharCode { get; set; }
        [XmlElement]
        public int Nominal { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string EngName { get; set; }
        public Valute()
        {
            Id = Guid.NewGuid();
        }
    }
}
