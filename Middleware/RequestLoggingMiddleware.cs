using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HackerNewsApi.Middleware;

public class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation($"Request started at {startTime:O} for {context.Request.Method} {context.Request.Path}");

        await next(context);

        var endTime = DateTime.UtcNow; var duration = endTime - startTime;
        _logger.LogInformation($"Request ended at {endTime:O} for {context.Request.Method} {context.Request.Path}. Duration: {duration.TotalMilliseconds} ms");
    }
}