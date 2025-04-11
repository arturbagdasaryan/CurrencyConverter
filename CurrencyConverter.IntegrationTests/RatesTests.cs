using System.Net.Http.Headers;
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

            // Act
            var response = await _client.GetAsync("/api/v1/rates/latest");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("USD");  // Check if the response contains USD rates
        }

        [TestMethod]
        public async Task GetRates_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var token = await GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/rates?invalidParameter=value");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
