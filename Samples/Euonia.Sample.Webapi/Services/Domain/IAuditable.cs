using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Sample.Domain;

/// <summary>
/// Represent the object has auditing properties with generic key type.
/// </summary>
public interface IAuditable : IAuditable<string>
{
}
