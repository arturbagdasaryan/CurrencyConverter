namespace CurrencyConverter.Api.Models
{
    public class HistoricalRatesResponse
    {
        public string BaseCurrency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<ExchangeRatesResponse> RatesByDate { get; set; }
    }
}
