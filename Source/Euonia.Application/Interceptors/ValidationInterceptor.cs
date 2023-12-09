using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.DynamicProxy;
using Nerosoft.Euonia.Validation;

namespace Nerosoft.Euonia.Application;

/// <inheritdoc />
public class ValidationInterceptor : IInterceptor
{
    /// <inheritdoc />
    public void Intercept(IInvocation invocation)
    {
        Intercept(invocation.Method, invocation.Arguments);
        invocation.Proceed();
    }

    private static void Intercept(MethodBase method, IReadOnlyList<object> args)
    {
        var parameters = method.GetParameters();

        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            var argument = args[index];

            if (!parameter.ParameterType.IsInstanceOfType(argument))
            {
                continue;
            }

            var notNullAttribute = parameter.GetCustomAttribute<NotNullAttribute>();
            if (notNullAttribute != null && argument == null)
            {
                throw new ValidationException($"Parameter '{parameter.Name}' is required in method '{method.Name}'.");
            }

            var validationAttribute = parameter.GetCustomAttribute<ValidationAttribute>();
            if (validationAttribute == null)
            {
                continue;
            }

            Validate(argument, parameter.ParameterType);
        }
    }

    private static void Validate(object argument, Type parameterType)
    {
        var method = typeof(Validator).GetMethod(nameof(Validator.Validate), BindingFlags.Static | BindingFlags.Public);
        if (method == null)
        {
            return;
        }

        method.MakeGenericMethod(parameterType).Invoke(null, new[] { argument });
    }
}