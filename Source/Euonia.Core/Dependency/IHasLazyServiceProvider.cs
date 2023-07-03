namespace Nerosoft.Euonia.Dependency;

/// <summary>
/// <see cref="IHasLazyServiceProvider"/>
/// </summary>
public interface IHasLazyServiceProvider
{
    /// <summary>
    /// 
    /// </summary>
    ILazyServiceProvider LazyServiceProvider { get; set; }
}