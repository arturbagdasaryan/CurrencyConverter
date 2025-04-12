using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CurrencyConverter.IntegrationTests
{
    public class ApiTestBase
    {
        protected readonly HttpClient _client;

        public ApiTestBase()
        {
            var factory = new WebApplicationFactory<CurrencyConverter.Api.Startup>();
            _client = factory.CreateClient();
        }

        protected async Task<string> GetJwtToken()
        {
            var loginRequest = new
            {
                email = "admin@currencyapi.com",
                password = "P@ssw0rd"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            return tokenResponse?.token;
        }

        public class TokenResponse
        {
            public string token { get; set; }
        }
    }
}
