using System.Diagnostics;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// Used to trace the method calls chain.
/// </summary>
public class TracingInterceptor : IInterceptor
{
	private readonly ILogger<TracingInterceptor> _logger;
	private readonly IRequestContextAccessor _contextAccessor;

	/// <summary>
	/// Initializes a new instance of the <see cref="TracingInterceptor"/> class.
	/// </summary>
	/// <param name="logger"></param>
	public TracingInterceptor(ILoggerFactory logger)
	{
		_logger = logger.CreateLogger<TracingInterceptor>();
	}

	/// <inheritdoc />
	public TracingInterceptor(ILoggerFactory logger, IRequestContextAccessor contextAccessor)
		: this(logger)
	{
		_contextAccessor = contextAccessor;
	}

	/// <inheritdoc />
	public void Intercept(IInvocation invocation)
	{
		if (_contextAccessor != null)
		{
			var traceInfoBuilder = new StringBuilder();
			var trace = new StackTrace();
			var frames = trace.GetFrames();
			foreach (var frame in frames)
			{
				var method = frame.GetMethod();
				if (method == null)
				{
					continue;
				}

				var className = method.DeclaringType?.FullName;
				traceInfoBuilder.AppendLine($" at {className}.{method.Name} in {frame.GetFileName()} ln:{frame.GetFileLineNumber()}");
			}

			_logger.LogDebug("TraceInfo: {TraceInfo}", traceInfoBuilder.ToString());
		}

		invocation.Proceed();
	}
}