using System.Text.Json;
using CurrencyConverter.Api.Models;

namespace CurrencyConverter.Api.Services
{
    public class FrankfurterCurrencyProvider : ICurrencyProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FrankfurterCurrencyProvider> _logger;

        public FrankfurterCurrencyProvider(HttpClient httpClient, ILogger<FrankfurterCurrencyProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ExchangeRatesResponse> GetLatestRatesAsync(string baseCurrency)
        {
            var url = $"https://api.frankfurter.app/latest?base={baseCurrency}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var rates = JsonSerializer.Deserialize<ExchangeRatesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return rates;
        }

        public async Task<HistoricalRatesResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate)
        {
            var url = $"https://api.frankfurter.app/{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base={baseCurrency}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            // Note: Depending on the actual API response you may need to transform the JSON to your HistoricalRatesResponse model.
            var historicalRates = JsonSerializer.Deserialize<HistoricalRatesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return historicalRates;
        }
    }
}
