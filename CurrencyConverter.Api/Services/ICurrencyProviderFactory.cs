namespace CurrencyConverter.Api.Services
{
    public interface ICurrencyProviderFactory
    {
        ICurrencyProvider GetProvider(string providerName);
    }
}
