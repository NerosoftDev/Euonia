using System.Net;
using System.Reflection;
using System.Security.Authentication;

namespace Nerosoft.Euonia.Hosting;

internal static class ExceptionExtensions
{
	extension(Exception exception)
	{
		public HttpStatusCode GetStatusCode()
		{
			return exception switch
			{
				HttpStatusException ex => ex.StatusCode,
				ValidationException => HttpStatusCode.BadRequest,
				AuthenticationException => HttpStatusCode.Unauthorized,
				UnauthorizedAccessException => HttpStatusCode.Forbidden,
				NotImplementedException => HttpStatusCode.NotImplemented,
				TimeoutException => HttpStatusCode.GatewayTimeout,
				AggregateException aggEx => aggEx.InnerExceptions.Count == 0 ? HttpStatusCode.InternalServerError : aggEx.InnerExceptions[0].GetStatusCode(),
				TargetInvocationException ex => ex.InnerException?.GetStatusCode() ?? HttpStatusCode.InternalServerError,
				_ => exception.GetType().GetCustomAttribute<HttpStatusCodeAttribute>()?.StatusCode ?? HttpStatusCode.InternalServerError
			};
		}

		public IReadOnlyDictionary<string, string[]> GetErrorDetails()
		{
			var ex = exception;
			while (true)
			{
				if (ex is AggregateException)
				{
					ex = ex.InnerException;
					continue;
				}

				if (ex is not ValidationException validationException)
				{
					return null;
				}

				return validationException.Errors
				                          .GroupBy(t => t.PropertyName ?? "_")
				                          .ToDictionary(t => t.Key, t => t.Select(x => x.ErrorMessage).ToArray());
			}
		}

		public string GetErrorMessage()
		{
			return exception switch
			{
				AggregateException ex => ex.InnerException == null ? ex.Message : ex.InnerException.GetErrorMessage(),
				TargetInvocationException ex => ex.InnerException == null ? ex.Message : ex.InnerException.GetErrorMessage(),
				_ => exception.Message
			};
		}
	}
}