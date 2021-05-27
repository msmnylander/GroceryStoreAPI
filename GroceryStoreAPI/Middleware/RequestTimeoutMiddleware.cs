using GroceryStoreAPI.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Middleware
{
    /// <summary>
    /// Implements a global query timeout.
    /// If the incoming request query parameters contain parameter "timeout" with a valid integer value,
    /// this middleware will inject a CancellationToken with the defined timeout (in milliseconds).
    /// If the request doesn't complete within the defined timeout, the token will cause the request
    /// to terminate.
    /// The minimum timeout is configured in the application configuration.
    /// </summary>
    public class RequestTimeoutMiddleware
    {
        public const string QUERY_TIMEOUT = "timeout";

        private readonly RequestDelegate _next;

        public RequestTimeoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IOptions<RequestTimeoutSettings> requestTimeoutSettings)
        {
            if (!context.Request.Query.ContainsKey(QUERY_TIMEOUT)
            || !int.TryParse(context.Request.Query[QUERY_TIMEOUT].ToString(), out int timeout))
            {
                await _next(context);
                return;
            }
            timeout = Math.Max(requestTimeoutSettings.Value.MinimumRequestTimeoutMs, timeout);
            using (var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted))
            {
                timeoutSource.CancelAfter(timeout);
                context.RequestAborted = timeoutSource.Token;
                await _next(context);
            }
        }
    }

    public static class RequestTimeoutMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTimeout(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<RequestTimeoutMiddleware>();
            return builder;
        }
    }

}
