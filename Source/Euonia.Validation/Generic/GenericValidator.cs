using System.Linq.Expressions;

namespace Nerosoft.Euonia.Validation;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class GenericValidator<TValue> : AbstractValidator<TValue>
{
    private readonly Func<TValue, string> _function;

    public GenericValidator(Expression<Func<TValue, string>> expression)
    {
        _function = expression.Compile();
    }

    public GenericValidator(Expression<Func<TValue, bool>> expression, string message)
    {
        var function = expression.Compile();
        _function = value => function(value) ? string.Empty : message;
    }

    public override bool Validate(TValue value)
    {
        Message = _function?.Invoke(value);
        return string.IsNullOrEmpty(Message);
    }
}