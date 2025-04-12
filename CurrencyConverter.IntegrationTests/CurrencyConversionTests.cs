using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CurrencyConverter.IntegrationTests
{
    [TestClass]
    public class CurrencyConversionTests : ApiTestBase
    {
        [TestMethod]
        public async Task ConvertCurrency_ValidRequest_ReturnsConvertedAmount()
        {
            // Arrange
            var fromCurrency = "USD";
            var toCurrency = "EUR";
            var amount = 100m;
            var token = await GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new 
            {
                sourceCurrency = fromCurrency,
                targetCurrency = toCurrency,
                amount = amount
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/rates/convert", request);

            // Assert
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CurrencyConversionResponse>();

            result.Should().NotBeNull();
            result!.ConvertedAmount.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task ConvertCurrency_InvalidCurrency_ReturnsBadRequest()
        {
            // Arrange
            var fromCurrency = "TRY";  // Invalid currency
            var toCurrency = "EUR";
            var amount = 100m;
            var token = await GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                sourceCurrency = fromCurrency,
                targetCurrency = toCurrency,
                amount = amount
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/rates/convert", request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrWhiteSpace();
            content.Should().Contain("Conversion involving one or more excluded currencies is not allowed.");
        }

    }
    public class CurrencyConversionResponse
    {
        public decimal ConvertedAmount { get; set; }
    }
}
