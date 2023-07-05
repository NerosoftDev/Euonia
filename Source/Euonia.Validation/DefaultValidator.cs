using FluentValidation;

namespace Nerosoft.Euonia.Validation;

public class DefaultValidator : IValidator
{
    private readonly IServiceProvider _service;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    public DefaultValidator(IServiceProvider service)
    {
        _service = service;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public void Validate<T>(T item) where T : class
    {
        var validator = (IValidator<T>)_service.GetService(typeof(IValidator<T>));
        var result = validator?.Validate(item);
        if (result == null)
        {
            return;
        }

        if (result.IsValid)
        {
            return;
        }

        var errors = result.Errors.Select(error => new ValidationResult(error.PropertyName, error.ErrorMessage));
        throw new ValidationException(string.Empty, errors);
    }
}