using MongoDB.Bson.Serialization;

namespace Nerosoft.Euonia.Repository.Mongo;

public class SequentialGuidValueGenerator : IIdGenerator, IValueGenerator<Guid>
{
    public Guid Generate()
    {
        return ObjectId.NewGuid(GuidType.SequentialAsString);
    }

    public object GenerateId(object container, object document)
    {
        return Generate();
    }

    public bool IsEmpty(object id)
    {
        return id == null || (Guid)id == Guid.Empty;
    }

    object IValueGenerator.Generate()
    {
        return Generate();
    }
}
