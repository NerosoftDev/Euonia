using System.Data;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia;

[Serializable]
public class DataNotFoundException : DataException
{
    private const string DEFAULT_MESSAGE = "Data not found";

    /// <summary>
    /// 
    /// </summary>
    public DataNotFoundException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public DataNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public DataNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <inheritdoc />
    protected DataNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
