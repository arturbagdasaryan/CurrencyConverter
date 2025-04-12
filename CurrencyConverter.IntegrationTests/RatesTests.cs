using System.Net.Http.Headers;
using System.Net.Http.Json;
using CurrencyConverter.Api.Models;
using FluentAssertions;

namespace CurrencyConverter.IntegrationTests
{
    [TestClass]
    public class RatesTests : ApiTestBase
    {
        [TestMethod]
        public async Task GetRates_ValidRequest_ReturnsRates()
        {
            // Arrange
            var token = await GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var baseCurrency = "USD";

            // Act
            var response = await _client.GetAsync($"/api/v1/rates/latest?baseCurrency={baseCurrency}");

            // Assert
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ExchangeRatesResponse>();
            result.Should().NotBeNull();
            result.Rates.Should().NotBeEmpty("there should be at least one exchange rate returned");
            result.Rates.Should().ContainKey("EUR", "EUR should be one of the returned currencies");
        }
    }
}
