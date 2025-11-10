using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Hosting;

/// <summary>
/// A middleware for handle exception.
/// </summary>
internal class ExceptionHandlingMiddleware : IMiddleware
{
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	/// <summary>
	/// Initialize a new instance of <see cref="ExceptionHandlingMiddleware"/>.
	/// </summary>
	/// <param name="logger"></param>
	public ExceptionHandlingMiddleware(ILoggerFactory logger)
	{
		_logger = logger.CreateLogger<ExceptionHandlingMiddleware>();
	}

	/// <inheritdoc />
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "{Message}", exception.Message);

			await HandleExceptionAsync(context, exception);
		}
	}

	private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
	{
		var statusCode = GetStatusCode(exception);

		var response = new
		{
			status = statusCode,
			message = GetMessage(exception),
			details = GetErrors(exception)
		};

		httpContext.Response.ContentType = "application/json";

		httpContext.Response.StatusCode = statusCode;

		return httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));


	}

	private static string GetMessage(Exception exception)
	{
		return exception switch
		{
			AggregateException ex => ex.InnerException == null ? ex.Message : GetMessage(ex.InnerException),
			TargetInvocationException ex => ex.InnerException == null ? ex.Message : GetMessage(ex.InnerException),
			_ => exception.Message
		};
	}

	private static int GetStatusCode(Exception exception)
	{
		return exception switch
		{
			HttpStatusException ex => (int)ex.StatusCode,
			AuthenticationException => StatusCodes.Status401Unauthorized,
			UnauthorizedAccessException => StatusCodes.Status403Forbidden,
			ValidationException => StatusCodes.Status400BadRequest,
			NotImplementedException => StatusCodes.Status501NotImplemented,
			AggregateException ex => GetStatusCode(ex.InnerException),
			TargetInvocationException ex => ex.InnerException == null ? StatusCodes.Status500InternalServerError : GetStatusCode(ex.InnerException),
			_ => (int)(exception.GetType().GetCustomAttribute<HttpStatusCodeAttribute>()?.StatusCode ?? HttpStatusCode.InternalServerError)
		};
	}

	private static IReadOnlyDictionary<string, string[]> GetErrors(Exception exception)
	{
		if (exception is AggregateException)
		{
			return GetErrors(exception.InnerException);
		}

		if (exception is not ValidationException ex)
		{
			return null;
		}

		return ex.Errors
				 .GroupBy(t => t.PropertyName)
				 .ToDictionary(t => t.Key, t => t.Select(x => x.ErrorMessage).ToArray());
	}
}