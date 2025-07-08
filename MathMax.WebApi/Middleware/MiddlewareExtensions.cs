using Microsoft.AspNetCore.Builder;

namespace MathMax.WebApi.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseErrorResponseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorResponseLoggingMiddleware>();
    }

    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}