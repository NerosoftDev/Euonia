namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message conversion delegate.
/// </summary>
public delegate object MessageConversionDelegate(object source, Type targetType);