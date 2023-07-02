using System.Diagnostics;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Application;

public class TracingInterceptor : IInterceptor
{
    private readonly ILogger<TracingInterceptor> _logger;
    private readonly IRequestContextAccessor _contextAccessor;

    public TracingInterceptor(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger<TracingInterceptor>();
    }

    public TracingInterceptor(ILoggerFactory logger, IRequestContextAccessor contextAccessor)
        : this(logger)
    {
        _contextAccessor = contextAccessor;
    }

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

                var className = method?.DeclaringType?.FullName;
                traceInfoBuilder.AppendLine($" at {className}.{method.Name} in {frame.GetFileName()} ln:{frame.GetFileLineNumber()}");
            }

            _logger.LogDebug("TraceInfo: {TraceInfo}", traceInfoBuilder.ToString());
        }

        invocation.Proceed();
    }
}