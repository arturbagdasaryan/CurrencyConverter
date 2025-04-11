using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConverter.Api.Infrastructure.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return value;
        }

        public void Set<T>(string key, T item, TimeSpan expiration)
        {
            _cache.Set(key, item, expiration);
        }
    }
}
