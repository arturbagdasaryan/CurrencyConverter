namespace CurrencyConverter.Api.Infrastructure.Caching
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T item, TimeSpan expiration);
    }
}
