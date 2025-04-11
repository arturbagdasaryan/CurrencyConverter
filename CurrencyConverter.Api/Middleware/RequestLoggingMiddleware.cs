using System.Diagnostics;

namespace CurrencyConverter.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var clientId = context.User?.Identity?.Name ?? "Unknown";
            var httpMethod = context.Request.Method;
            var endpoint = context.Request.Path;
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation("Request from {ClientIP} with ClientId {ClientId} - {HttpMethod} {Endpoint} responded {StatusCode} in {ElapsedMilliseconds}ms",
                clientIp, clientId, httpMethod, endpoint, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
