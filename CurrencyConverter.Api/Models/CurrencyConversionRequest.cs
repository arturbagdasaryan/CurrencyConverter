namespace CurrencyConverter.Api.Models
{
    public class CurrencyConversionRequest
    {
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal Amount { get; set; }
    }
}
