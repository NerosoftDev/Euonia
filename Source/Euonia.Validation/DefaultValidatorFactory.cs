namespace Nerosoft.Euonia.Validation;

public class DefaultValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    public DefaultValidatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IValidator Create()
    {
        return new DefaultValidator(_serviceProvider);
    }
}