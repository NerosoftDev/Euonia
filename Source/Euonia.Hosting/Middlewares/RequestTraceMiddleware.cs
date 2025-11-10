using Microsoft.AspNetCore.Http;

namespace Nerosoft.Euonia.Hosting;

/// <summary>
/// A middleware for trace http request.
/// </summary>
internal class RequestTraceMiddleware
{
	private readonly RequestDelegate _next;

	/// <summary>
	/// Initialize a new instance of <see cref="RequestTraceMiddleware"/>.
	/// </summary>
	/// <param name="next"></param>
	public RequestTraceMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	/// <summary>
	/// Handle the request.
	/// </summary>
	/// <param name="context">The current HttpContext instance.</param>
	/// <returns></returns>
	public async Task InvokeAsync(HttpContext context)
	{
		context.Response.Headers.Append("x-request-trace-id", context.TraceIdentifier);

		await _next(context);
	}
}