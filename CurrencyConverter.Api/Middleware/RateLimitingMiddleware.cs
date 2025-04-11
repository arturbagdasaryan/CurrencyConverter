using System.Collections.Concurrent;

namespace CurrencyConverter.Api.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static ConcurrentDictionary<string, RateLimitInfo> _clients = new();

        // Configuration: 100 requests per minute
        private readonly int _maxRequests = 100;
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = clientIp; // You can extend this key with clientId if needed

            var rateLimitInfo = _clients.GetOrAdd(key, new RateLimitInfo { RequestCount = 0, WindowStart = DateTime.UtcNow });

            lock (rateLimitInfo)
            {
                if (DateTime.UtcNow - rateLimitInfo.WindowStart > _timeWindow)
                {
                    rateLimitInfo.RequestCount = 0;
                    rateLimitInfo.WindowStart = DateTime.UtcNow;
                }

                rateLimitInfo.RequestCount++;

                if (rateLimitInfo.RequestCount > _maxRequests)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    return;
                }
            }

            await _next(context);
        }

        private class RateLimitInfo
        {
            public int RequestCount { get; set; }
            public DateTime WindowStart { get; set; }
        }
    }
}
