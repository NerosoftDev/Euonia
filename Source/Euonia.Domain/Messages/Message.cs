namespace Nerosoft.Euonia.Domain;

/// <summary>
/// A abstract implement of <see cref="IMessage"/>.
/// </summary>
public abstract class Message : IMessage
{
    /// <summary>
    /// The message type key
    /// </summary>
    public const string MessageTypeKey = "$nerosoft.euonia:message.type";

    /// <summary>
    /// Initializes a new instance of <see cref="Message"/>.
    /// </summary>
    protected Message()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        Metadata[MessageTypeKey] = GetType().AssemblyQualifiedName;
    }

    /// <summary>
    /// Gets or sets the identifier of the message.
    /// </summary>
    /// <value>
    /// The identifier of the message.
    /// </value>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets or sets the timestamp that describes when the message occurs.
    /// </summary>
    /// <value>
    /// The timestamp that describes when the message occurs.
    /// </value>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Gets a <see cref="MessageMetadata"/> instance that contains the metadata information of the message.
    /// </summary>
    public MessageMetadata Metadata { get; } = new();

    /// <summary>
    /// Gets the .NET CLR assembly qualified name of the message.
    /// </summary>
    /// <returns>
    /// The assembly qualified name of the message.
    /// </returns>
    public string GetTypeName() => Metadata[MessageTypeKey] as string;

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString() => Id.ToString();

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equals to this instance.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="object"/> to compare with this instance.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="object"/> is equals to this instance; otherwise <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is Message message && message.Id == Id;
    }

    /// <summary>
    /// Get hash code of this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        return GetHashCode(Id.GetHashCode(), Timestamp.GetHashCode());
    }

    private static int GetHashCode(params int[] hashCodesForProperties)
    {
        unchecked
        {
            return hashCodesForProperties.Aggregate(23, (current, code) => current * 29 + code);
        }
    }
}