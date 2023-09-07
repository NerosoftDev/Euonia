namespace Nerosoft.Euonia.Validation;

/// <summary>
/// The default validator factory.
/// </summary>
public class DefaultValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultValidatorFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public DefaultValidatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a new validator.
    /// </summary>
    /// <returns></returns>
    public IValidator Create()
    {
        return new DefaultValidator(_serviceProvider);
    }
}