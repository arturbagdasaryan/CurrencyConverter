using CurrencyConverter.Api.Models;

namespace CurrencyConverter.Api.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly ICurrencyProviderFactory _providerFactory;
        private readonly Infrastructure.Caching.ICacheService _cacheService;
        private readonly ILogger<CurrencyConversionService> _logger;

        public CurrencyConversionService(
            ICurrencyProviderFactory providerFactory,
            Infrastructure.Caching.ICacheService cacheService,
            ILogger<CurrencyConversionService> logger)
        {
            _providerFactory = providerFactory;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ExchangeRatesResponse> GetLatestRatesAsync(string baseCurrency)
        {
            // Check cache first
            string cacheKey = $"latest_{baseCurrency}";
            var cachedRates = _cacheService.Get<ExchangeRatesResponse>(cacheKey);
            if (cachedRates != null)
            {
                return cachedRates;
            }

            // Get provider via factory (currently returns Frankfurter provider)
            var provider = _providerFactory.GetProvider("Frankfurter");
            var rates = await provider.GetLatestRatesAsync(baseCurrency);

            // Cache the result (e.g., for 10 minutes)
            _cacheService.Set(cacheKey, rates, TimeSpan.FromMinutes(10));

            return rates;
        }

        public async Task<decimal> ConvertCurrencyAsync(CurrencyConversionRequest request)
        {
            var ratesResponse = await GetLatestRatesAsync(request.SourceCurrency);
            if (ratesResponse.Rates.TryGetValue(request.TargetCurrency.ToUpper(), out decimal rate))
            {
                return request.Amount * rate;
            }
            throw new ArgumentException("Conversion rate not found.");
        }

        public async Task<HistoricalRatesResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            // Get full historical data from the provider
            var provider = _providerFactory.GetProvider("Frankfurter");
            var fullResponse = await provider.GetHistoricalRatesAsync(baseCurrency, startDate, endDate);

            // Perform simple in-memory pagination
            var totalRecords = fullResponse.RatesByDate.Count;
            var pagedData = fullResponse.RatesByDate
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new HistoricalRatesResponse
            {
                BaseCurrency = baseCurrency,
                StartDate = startDate,
                EndDate = endDate,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                RatesByDate = pagedData
            };
        }
    }
}
