namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The command execution status definition.
/// </summary>
public enum CommandStatus
{
    /// <summary>
    /// Represents the command was successfully executed.
    /// </summary>
    Succeed,

    /// <summary>
    /// Represents the command execution was canceled.
    /// </summary>
    Canceled,

    /// <summary>
    /// Represents the specified data was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// Represents the command execution 
    /// </summary>
    Unauthorized,

    /// <summary>
    /// 
    /// </summary>
    Forbidden,

    /// <summary>
    /// 
    /// </summary>
    Timeout,

    /// <summary>
    /// 
    /// </summary>
    Internal,

    /// <summary>
    /// 
    /// </summary>
    Unimplemented,

    /// <summary>
    /// 
    /// </summary>
    Unavailable,

    /// <summary>
    /// 
    /// </summary>
    NotSupported,

    /// <summary>
    /// 
    /// </summary>
    Invalid
}