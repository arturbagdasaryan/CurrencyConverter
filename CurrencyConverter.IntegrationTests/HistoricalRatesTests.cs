using CurrencyConverter.Api.Models;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CurrencyConverter.IntegrationTests
{
    [TestClass]
    public class HistoricalRatesTests : ApiTestBase
    {
        [TestMethod]
        public async Task GetHistoricalRates_ValidRequest_ReturnsPagedHistoricalRates()
        {
            // Arrange
            var token = await GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var baseCurrency = "USD";
            var startDate = new DateTime(2025, 04, 01);
            var endDate = new DateTime(2025, 04, 10);
            var pageNumber = 1;
            var pageSize = 5;

            var url = $"/api/v1/rates/historical?baseCurrency={baseCurrency}" +
                      $"&startDate={startDate:yyyy-MM-dd}" +
                      $"&endDate={endDate:yyyy-MM-dd}" +
                      $"&pageNumber={pageNumber}&pageSize={pageSize}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<HistoricalRatesResponse>();

            result.Should().NotBeNull();
            result!.Base.Should().Be("USD");
            result.Start_Date.Should().Be(startDate);
            result.End_Date.Should().Be(endDate);
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.Rates.Should().NotBeNullOrEmpty();
            result.Rates.First().Value.Should().ContainKey("EUR");
        }
    }
}