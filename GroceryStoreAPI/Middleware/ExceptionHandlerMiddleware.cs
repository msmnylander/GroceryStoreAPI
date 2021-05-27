using GroceryStoreAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace GroceryStoreAPI.Middleware {
    /// <summary>
    /// Implements a global unhandled request exception handler.
    /// </summary>
    public static class ExceptionHandlerMiddleware
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/problem+json";
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionFeature != null)
                    {
                        var problem = new ProblemDetails
                        {
                            Status = StatusCodes.Status500InternalServerError,
                            Title = "An error occurred",
                            Detail = exceptionFeature.Error.ToClientMessage()
                        };
                        var traceId = context.TraceIdentifier;
                        if (traceId != null)
                        {
                            problem.Extensions["traceId"] = traceId;
                        }
                        logger.LogError($"Exception: {exceptionFeature.Error} Trace Id: {traceId}");
                        var stream = context.Response.Body;
                        await JsonSerializer.SerializeAsync(stream, problem);
                    }
                });
            });
        }
    }
}
