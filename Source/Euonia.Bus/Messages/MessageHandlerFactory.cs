namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The delegate to create message handler.
/// </summary>
public delegate MessageHandler MessageHandlerFactory(IServiceProvider provider);