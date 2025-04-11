namespace CurrencyConverter.Api.Services
{
    public class CurrencyProviderFactory : ICurrencyProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrencyProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICurrencyProvider GetProvider(string providerName)
        {
            // For now, we assume only Frankfurter is supported.
            if (providerName.Equals("Frankfurter", StringComparison.OrdinalIgnoreCase))
            {
                return _serviceProvider.GetRequiredService<ICurrencyProvider>();
            }

            throw new System.ArgumentException("Unsupported currency provider.");
        }
    }
}
