using System;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangeRateService.DAL
{
    public class CurrencyRate
    {
        [Key]
        public Guid Id { get; set; }
        public Valute Valute { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }
        public CurrencyRate()
        {
            Id = Guid.NewGuid();
        }
    }
}
