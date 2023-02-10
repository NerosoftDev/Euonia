namespace Nerosoft.Euonia.Validation;

/// <summary>
/// 
/// </summary>
public interface IValidatorFactory
{
    /// <summary>
    /// Create a new IEntityValidator
    /// </summary>
    /// <returns>IEntityValidator</returns>
    IValidator Create();
}