using System.Runtime.Serialization;

namespace Nerosoft.Euonia;

[Serializable]
public class InvalidValueException : Exception
{
    public InvalidValueException()
    {
    }

    public InvalidValueException(string message)
        : base(message)
    {
    }

    public InvalidValueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected InvalidValueException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}
