namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The command execution response object.
/// </summary>
public class CommandResponse
{
    /// <summary>
    /// Initialize a new instance of <see cref="CommandResponse"/>.
    /// </summary>
    /// <param name="commandId">The unique id of command to execute.</param>
    public CommandResponse(string commandId)
    {
        CommandId = commandId;
    }

    /// <summary>
    /// Gets the command id.
    /// </summary>
    /// <value>The unique id of a <see cref="Command"/> instance.</value>
    public string CommandId { get; }

    /// <summary>
    /// Gets or sets the command execution status
    /// </summary>
    public CommandStatus Status { get; set; }

    /// <summary>
    /// Gets whether the command has executed successfully.
    /// </summary>
    public bool IsSuccess => Status == CommandStatus.Succeed;

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the error type name.
    /// </summary>
    public Exception Error { get; set; }
}

/// <summary>
/// The command execution response object with return result of <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The return result type.</typeparam>
public class CommandResponse<TResult> : CommandResponse
{
    /// <summary>
    /// Initialize a new instance of <see cref="CommandResponse{TResult}"/>.
    /// </summary>
    /// <param name="commandId">The unique id of command to execute.</param>
    public CommandResponse(string commandId)
        : base(commandId)
    {
    }

    /// <summary>
    /// Gets or sets the return result.
    /// </summary>
    public TResult Result { get; set; }
}