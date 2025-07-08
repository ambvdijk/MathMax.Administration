using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MathMax.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Proceed to the next middleware
        }
        catch (Exception ex)
        {
            var traceId = Guid.NewGuid().ToString();

            _logger.LogError(ex,
                "Unhandled exception occurred while processing {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                traceId
            );

            await HandleExceptionAsync(context, traceId);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, string traceId)
    {
        context.Response.Clear();
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var errorPayload = new
        {
            error = "An unexpected error occurred. Please contact support.",
            traceId
        };

        var result = JsonSerializer.Serialize(errorPayload);
        return context.Response.WriteAsync(result);
    }
}
