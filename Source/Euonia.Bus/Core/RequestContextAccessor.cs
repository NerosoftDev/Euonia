namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The delegate to get the <see cref="RequestContext"/> instance of current request.
/// </summary>
/// <returns></returns>
public delegate RequestContext RequestContextAccessor(IServiceProvider provider);