using Microsoft.AspNetCore.Http;

namespace Nerosoft.Euonia.Hosting;

public class RequestTraceMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTraceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Add("x-request-trace-id", context.TraceIdentifier);

        await _next(context);
    }
}