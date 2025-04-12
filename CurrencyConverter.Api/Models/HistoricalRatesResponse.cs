namespace CurrencyConverter.Api.Models
{
    public class HistoricalRatesResponse
    {
        public double Amount { get; set; }
        public string Base { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Dictionary<DateTime, Dictionary<string, double>> Rates { get; set; }
    }
}
