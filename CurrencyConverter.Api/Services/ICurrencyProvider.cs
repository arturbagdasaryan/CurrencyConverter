using CurrencyConverter.Api.Models;

namespace CurrencyConverter.Api.Services
{
    public interface ICurrencyProvider
    {
        Task<ExchangeRatesResponse> GetLatestRatesAsync(string baseCurrency);
        Task<HistoricalRatesResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate);
    }
}
