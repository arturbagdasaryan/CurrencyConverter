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
                username = "admin@currencyapi.com", // Replace with actual test credentials
                password = "P@ssw0rd" // Replace with actual test password
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            return tokenResponse?.Token;
        }

        public class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}
