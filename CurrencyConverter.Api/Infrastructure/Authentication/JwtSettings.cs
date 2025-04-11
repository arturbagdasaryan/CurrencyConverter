namespace CurrencyConverter.Api.Infrastructure.Authentication
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int ExpiryInMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
