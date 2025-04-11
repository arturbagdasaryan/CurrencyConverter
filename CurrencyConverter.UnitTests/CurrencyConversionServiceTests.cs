using Moq;
using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Services;
using CurrencyConverter.Api.Infrastructure.Caching;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.UnitTests.Services
{
    [TestClass]
    public class CurrencyConversionServiceTests
    {
        private Mock<ICurrencyProviderFactory> _mockFactory;
        private Mock<ICurrencyProvider> _mockProvider;
        private Mock<ICacheService> _mockCache;
        private Mock<ILogger<CurrencyConversionService>> _mockLogger;
        private CurrencyConversionService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockFactory = new Mock<ICurrencyProviderFactory>();
            _mockProvider = new Mock<ICurrencyProvider>();
            _mockCache = new Mock<ICacheService>();
            _mockLogger = new Mock<ILogger<CurrencyConversionService>>();

            _mockFactory.Setup(f => f.GetProvider("Frankfurter")).Returns(_mockProvider.Object);

            _service = new CurrencyConversionService(
                _mockFactory.Object,
                _mockCache.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task GetLatestRatesAsync_ReturnsFromCache_IfExists()
        {
            // Arrange
            var cached = new ExchangeRatesResponse
            {
                BaseCurrency = "EUR",
                Rates = new Dictionary<string, decimal> { { "USD", 1.1m } },
                Date = DateTime.UtcNow
            };

            _mockCache.Setup(c => c.Get<ExchangeRatesResponse>("latest_EUR"))
                      .Returns(cached);

            // Act
            var result = await _service.GetLatestRatesAsync("EUR");

            // Assert
            Assert.AreEqual(cached, result);
            _mockProvider.Verify(p => p.GetLatestRatesAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLatestRatesAsync_FetchesFromProvider_IfNotCached()
        {
            // Arrange
            var fetched = new ExchangeRatesResponse
            {
                BaseCurrency = "EUR",
                Rates = new Dictionary<string, decimal> { { "USD", 1.1m } },
                Date = DateTime.UtcNow
            };

            _mockCache.Setup(c => c.Get<ExchangeRatesResponse>("latest_EUR"))
                      .Returns((ExchangeRatesResponse)null);

            _mockProvider.Setup(p => p.GetLatestRatesAsync("EUR"))
                         .ReturnsAsync(fetched);

            // Act
            var result = await _service.GetLatestRatesAsync("EUR");

            // Assert
            Assert.AreEqual(fetched, result);
            _mockCache.Verify(c => c.Set("latest_EUR", fetched, TimeSpan.FromMinutes(10)), Times.Once);
        }

        [TestMethod]
        public async Task ConvertCurrencyAsync_ReturnsCorrectValue()
        {
            // Arrange
            var response = new ExchangeRatesResponse
            {
                BaseCurrency = "EUR",
                Rates = new Dictionary<string, decimal> { { "USD", 1.2m } },
                Date = DateTime.UtcNow
            };

            _mockCache.Setup(c => c.Get<ExchangeRatesResponse>("latest_EUR"))
                      .Returns(response);

            var request = new CurrencyConversionRequest
            {
                SourceCurrency = "EUR",
                TargetCurrency = "USD",
                Amount = 100
            };

            // Act
            var result = await _service.ConvertCurrencyAsync(request);

            // Assert
            Assert.AreEqual(120m, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ConvertCurrencyAsync_Throws_IfTargetRateMissing()
        {
            // Arrange
            var response = new ExchangeRatesResponse
            {
                BaseCurrency = "EUR",
                Rates = new Dictionary<string, decimal>(), // No USD
                Date = DateTime.UtcNow
            };

            _mockCache.Setup(c => c.Get<ExchangeRatesResponse>("latest_EUR"))
                      .Returns(response);

            var request = new CurrencyConversionRequest
            {
                SourceCurrency = "EUR",
                TargetCurrency = "USD",
                Amount = 100
            };

            // Act
            await _service.ConvertCurrencyAsync(request);
        }

        [TestMethod]
        public async Task GetHistoricalRatesAsync_ReturnsPagedData()
        {
            // Arrange
            var allRates = Enumerable.Range(1, 10).Select(i => new KeyValuePair<DateTime, Dictionary<string, decimal>>(
                new DateTime(2024, 01, i),
                new Dictionary<string, decimal> { { "USD", 1.0m + i } }
            )).ToList();

            // Convert the dictionary into a list of ExchangeRatesResponse
            var exchangeRatesList = allRates.Select(kvp => new ExchangeRatesResponse
            {
                BaseCurrency = "EUR",
                Date = kvp.Key,
                Rates = kvp.Value
            }).ToList();

            // Set up the expected full response with historical data
            var fullResponse = new HistoricalRatesResponse
            {
                BaseCurrency = "EUR",
                RatesByDate = exchangeRatesList
            };

            // Mock the provider call to return the full historical response
            _mockProvider.Setup(p => p.GetHistoricalRatesAsync("EUR", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                         .ReturnsAsync(fullResponse);

            // Act
            var result = await _service.GetHistoricalRatesAsync("EUR", DateTime.UtcNow.AddDays(-10), DateTime.UtcNow, pageNumber: 2, pageSize: 3);

            // Assert
            Assert.AreEqual(3, result.RatesByDate.Count);  // Check pagination (page 2 with 3 items)
            Assert.AreEqual(10, result.TotalRecords);      // Total records should be 10
            Assert.AreEqual(2, result.PageNumber);         // Page number should be 2
            Assert.AreEqual(3, result.PageSize);           // Page size should be 3
        }

    }
}
