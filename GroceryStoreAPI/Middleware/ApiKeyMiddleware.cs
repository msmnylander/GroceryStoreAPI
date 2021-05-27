using GroceryStoreAPI.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Middleware
{
    /// <summary>
    /// Implements a trivial API KEY authorization.
    /// Adds a global hook into the request pipeline that equires that every request to the API has an "ApiKey" 
    /// header with a string that matches a key configured in the application settings.
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        public const string APIKEY = "ApiKey";
        public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context, IOptions<ApiKeySettings> apiKeySettings)
        {
            if (!context.Request.Headers.TryGetValue(APIKEY, out var apiKey) || apiKeySettings.Value.Equals(apiKey))
            {
                await Fail(context);
                return;
            }
            await _next(context);
        }

        private async Task Fail(HttpContext context)
        {
            var remoteIp = context.Request.HttpContext.Connection?.RemoteIpAddress?.ToString();
            _logger.LogInformation($"Unauthorized request from {remoteIp}.");
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Invalid or missing API key"
            };
            problem.Extensions["remoteIp"] = remoteIp;
            var stream = context.Response.Body;
            await JsonSerializer.SerializeAsync(stream, problem);
        }

        private readonly ILogger<ApiKeyMiddleware> _logger;
    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder RequireApiKey(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ApiKeyMiddleware>();
            return builder;
        }
    }

}
