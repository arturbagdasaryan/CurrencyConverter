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
        public async Task GetHistoricalRatesAsync_ShouldReturnCorrectlyPagedData()
        {
            // Arrange
            var allRates = Enumerable.Range(1, 10).Select(i =>
                             new KeyValuePair<DateTime, Dictionary<string, double>>(
                                 new DateTime(2024, 01, i),
                                 new Dictionary<string, double> { { "USD", (double)(1.0m + i) } }
                             )
                         ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


            var fullResponse = new HistoricalRatesResponse
            {
                Base = "EUR",
                Start_Date = new DateTime(2024, 01, 1),
                End_Date = new DateTime(2024, 01, 10),
                Rates = allRates,
                TotalRecords = 10,
                PageNumber = 1,
                PageSize = 10
            };

            _mockProvider
                .Setup(p => p.GetHistoricalRatesAsync("EUR", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(fullResponse);

            var expectedPage = 2;
            var expectedPageSize = 3;

            // Act
            var result = await _service.GetHistoricalRatesAsync("EUR", DateTime.UtcNow.AddDays(-10), DateTime.UtcNow, expectedPage, expectedPageSize);

            // Assert
            Assert.AreEqual(expectedPageSize, result.Rates.Count, "Should return correct number of items per page");
            Assert.AreEqual(fullResponse.Rates.Count, result.TotalRecords, "Total records count should match");
            Assert.AreEqual(expectedPage, result.PageNumber, "Should return the correct page number");
            Assert.AreEqual(expectedPageSize, result.PageSize, "Should return the correct page size");

            // Optional: Check specific dates or currency keys if desired
            var expectedDates = allRates.Keys.Skip((expectedPage - 1) * expectedPageSize).Take(expectedPageSize).ToList();
            CollectionAssert.AreEqual(expectedDates, result.Rates.Keys.ToList(), "Returned dates should match expected page slice");
        }
    }
}
