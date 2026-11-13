using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LeVietTravel.Common.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate _next, ILogger<RequestLoggingMiddleware> logger)
    {
        this._next = _next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;

        _logger.LogInformation("--> [{Method}] {Path}{QueryString}", 
            request.Method, request.Path, request.QueryString);

        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation("<-- [{Method}] {Path} - Status: {StatusCode} in {ElapsedMilliseconds}ms", 
            request.Method, request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
