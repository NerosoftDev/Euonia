using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

public class MessageHandledEventArgs : EventArgs
{
    public MessageHandledEventArgs(IMessage message)
    {
        Message = message;
    }

    /// <summary>
    /// Gets the handle message.
    /// </summary>
    public IMessage Message { get; }

    /// <summary>
    /// Gets the handler type.
    /// </summary>
    public Type HandlerType { get; internal set; }
}