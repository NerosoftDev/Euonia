namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The delegate to resolve the <see cref="CommandStatus"/> from the <see cref="Exception"/>.
/// </summary>
public delegate CommandStatus CommandStatusResolve(Exception exception);