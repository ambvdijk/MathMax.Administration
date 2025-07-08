using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MathMax.WebApi.Middleware;

public class ErrorResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorResponseLoggingMiddleware> _logger;

    public ErrorResponseLoggingMiddleware(RequestDelegate next, ILogger<ErrorResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // Only log if response has a client or server error status
        if (context.Response.StatusCode >= 400)
        {
            LogErrorResponse(context);
        }
    }

    private void LogErrorResponse(HttpContext context)
    {
        try
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            var statusCode = context.Response.StatusCode;

            var logLevel = GetLogLevel(statusCode);
            var message = GetStatusCodeMessage(statusCode, method, path);

            _logger.Log(logLevel, "{Message}", message);
        }
        catch (Exception ex)
        {
            // Swallow logging errors to avoid interfering with the response pipeline
            _logger.LogError(ex, "Error occurred while logging an error response. StatusCode >= 400");
        }
    }

    private static string GetStatusCodeMessage(int statusCode, string method, string path)
    {
        var reason = ReasonPhrases.GetReasonPhrase(statusCode) ?? "Unknown Error";
        return $"{statusCode} {reason}: {method} {path}";
    }

    private static LogLevel GetLogLevel(int statusCode)
    {
        if (statusCode >= 500)
        {
            return LogLevel.Error;
        }

        if (statusCode >= 400)
        {
            return LogLevel.Warning;
        }

        return LogLevel.Information;
    }
}

