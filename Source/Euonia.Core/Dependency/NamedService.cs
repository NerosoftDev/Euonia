namespace Nerosoft.Euonia.Dependency;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TService"></typeparam>
public delegate TService NamedService<out TService>(string name);