namespace Nerosoft.Euonia.Dependency;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface IObjectAccessor<out TValue>
{
    /// <summary>
    /// 
    /// </summary>
    TValue Value { get; }
}