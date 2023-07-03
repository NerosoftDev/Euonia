using System.ComponentModel;
using MongoDB.Bson.Serialization;

namespace Nerosoft.Euonia.Repository.Mongo;

public class ObjectIdSerializer : IBsonSerializer
{
    public ObjectIdSerializer(Type valueType)
    {
        ValueType = valueType;
    }

    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var id = context.Reader.ReadObjectId().ToString();
        return TypeDescriptor.GetConverter(ValueType).ConvertFrom(id);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        var data = MongoDB.Bson.ObjectId.Empty;
        context.Writer.WriteObjectId(data);
    }

    public Type ValueType { get; }
}

public class ObjectIdSerializer<T> : ObjectIdSerializer, IBsonSerializer<T>
{
    public ObjectIdSerializer()
        : base(typeof(T))
    {
    }

    public new T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return (T)base.Deserialize(context, args);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        base.Serialize(context, args, value);
    }
}