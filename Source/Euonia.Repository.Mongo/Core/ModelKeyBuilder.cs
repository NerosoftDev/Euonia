using MongoDB.Bson.Serialization;

namespace Nerosoft.Euonia.Repository.Mongo;

public class ModelKeyBuilder
{
    internal ModelKeyBuilder(string key, Type type)
    {
        Key = key;
        Type = type;
    }

    internal string Key { get; }

    internal Type Type { get; }

    internal IIdGenerator IdGenerator { get; private set; }

    internal bool AutoGenerateId { get; private set; } = true;

    public ModelKeyBuilder GenerateOnInsert(bool value)
    {
        AutoGenerateId = value;
        return this;
    }

    public ModelKeyBuilder UseValueGenerator<TGenerator>()
        where TGenerator : class, IIdGenerator, new()
    {
        IdGenerator = Singleton<TGenerator>.Get(() => new());
        return this;
    }
}