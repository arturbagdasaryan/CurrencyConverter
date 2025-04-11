namespace CurrencyConverter.Api.Models
{
    public class ExchangeRatesResponse
    {
        public string BaseCurrency { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
