using FluentAssertions;
using System.Net.Http.Headers;

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

            // Act
            var response = await _client.GetAsync($"/api/v1/rates/convert?sourceCurrency={fromCurrency}&targetCurrency={toCurrency}&amount={amount}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("EUR");  // Check if the response contains the EUR currency
        }

        [TestMethod]
        public async Task ConvertCurrency_InvalidCurrency_ReturnsBadRequest()
        {
            // Arrange
            var fromCurrency = "XYZ";  // Invalid currency
            var toCurrency = "EUR";
            var amount = 100m;
            var token = await GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"/api/v1/rates/convert?sourceCurrency={fromCurrency}&targetCurrency={toCurrency}&amount={amount}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
