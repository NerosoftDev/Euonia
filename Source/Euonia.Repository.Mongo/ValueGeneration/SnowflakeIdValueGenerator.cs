using MongoDB.Bson.Serialization;

namespace Nerosoft.Euonia.Repository.Mongo;

public class SnowflakeIdValueGenerator : IIdGenerator, IValueGenerator<long>
{
    public long Generate()
    {
        var id = ObjectId.NewSnowflake();
        return id;
    }

    public object GenerateId(object container, object document)
    {
        return Generate();
    }

    public bool IsEmpty(object id)
    {
        return id == null || (long)id == 0;
    }

    object IValueGenerator.Generate()
    {
        return Generate();
    }
}
