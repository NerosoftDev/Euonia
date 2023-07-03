using System.Text.Json;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Application;

/// <inheritdoc />
public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public LoggingInterceptor(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger<LoggingInterceptor>();
    }

    /// <inheritdoc />
    public void Intercept(IInvocation invocation)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            try
            {
                var arguments = GetArguments(invocation);

                _logger.LogDebug("Method: {Method}, Arguments: {Arguments}", invocation.Method.Name, JsonSerializer.Serialize(arguments));
            }
            catch
            {
                _logger.LogDebug("Method: {Method}, Arguments: {Arguments}", invocation.Method.Name, "Error while logging arguments");
            }
        }

        try
        {
            invocation.Proceed();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while executing method: {Method}, {Message}", invocation.Method.Name, exception.Message);
            throw;
        }
    }

    private static Dictionary<string, object> GetArguments(IInvocation invocation)
    {
        var parameters = invocation.Method.GetParameters();
        var dictionary = new Dictionary<string, object>();
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            if (string.IsNullOrEmpty(parameter.Name))
            {
                continue;
            }

            dictionary.Add(parameter.Name, invocation.Arguments[index]);
        }

        return dictionary;
    }
}