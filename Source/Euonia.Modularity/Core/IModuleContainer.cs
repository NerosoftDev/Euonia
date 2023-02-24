namespace Nerosoft.Euonia.Modularity;

public interface IModuleContainer
{
    IReadOnlyList<IModuleDescriptor> Modules { get; }
}
