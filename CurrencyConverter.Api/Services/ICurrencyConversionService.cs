using CurrencyConverter.Api.Models;

namespace CurrencyConverter.Api.Services
{
    public interface ICurrencyConversionService
    {
        Task<ExchangeRatesResponse> GetLatestRatesAsync(string baseCurrency);
        Task<decimal> ConvertCurrencyAsync(CurrencyConversionRequest request);
        Task<HistoricalRatesResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    }
}
