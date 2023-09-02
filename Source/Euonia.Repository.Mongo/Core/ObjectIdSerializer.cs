using System.ComponentModel;
using MongoDB.Bson.Serialization;

namespace Nerosoft.Euonia.Repository.Mongo;

/// <summary>
/// The class used to serialize a <see cref="ObjectId"/> to a <see cref="string"/>.
/// </summary>
public class ObjectIdSerializer : IBsonSerializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectIdSerializer"/> class.
    /// </summary>
    /// <param name="valueType"></param>
    public ObjectIdSerializer(Type valueType)
    {
        ValueType = valueType;
    }

    /// <inheritdoc />
    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var id = context.Reader.ReadObjectId().ToString();
        return TypeDescriptor.GetConverter(ValueType).ConvertFrom(id);
    }

    /// <inheritdoc />
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        var data = MongoDB.Bson.ObjectId.Empty;
        context.Writer.WriteObjectId(data);
    }

    /// <inheritdoc />
    public Type ValueType { get; }
}

/// <summary>
/// The class used to serialize a <see cref="ObjectId"/> to a <see cref="string"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectIdSerializer<T> : ObjectIdSerializer, IBsonSerializer<T>
{
	/// <inheritdoc />
	public ObjectIdSerializer()
        : base(typeof(T))
    {
    }

	/// <inheritdoc />
	public new T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return (T)base.Deserialize(context, args);
    }

	/// <inheritdoc />
	public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        base.Serialize(context, args, value);
    }
}