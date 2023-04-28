namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Class MessageRepliedEventArgs.
/// Implements the <see cref="EventArgs" />
/// </summary>
/// <seealso cref="EventArgs" />
public class MessageRepliedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageRepliedEventArgs"/> class.
    /// </summary>
    /// <param name="result">The result.</param>
    public MessageRepliedEventArgs(object result)
    {
        Result = result;
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <value>The result.</value>
    public object Result { get; }
}