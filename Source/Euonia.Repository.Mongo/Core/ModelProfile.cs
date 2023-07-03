using System;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using Nerosoft.Euonia.Reflection;

namespace Nerosoft.Euonia.Repository.Mongo;

public class ModelProfile
{
    private ModelKeyBuilder _keyBuilder;
    internal string KeyName => _keyBuilder?.Key;

    internal Type KeyType => _keyBuilder?.Type;

    internal string CollectionName { get; private set; }

    internal bool AutoMapProperty { get; private set; } = true;

    /// <summary>
    /// 是否自动生成Id
    /// </summary>
    internal bool AutoGenerateId => _keyBuilder?.AutoGenerateId ?? true;

    internal IIdGenerator IdGenerator => _keyBuilder?.IdGenerator;

    internal bool? BypassDocumentValidation { get; set; }

    public ModelKeyBuilder HasKey(string key, Type type = null)
    {
        _keyBuilder = new ModelKeyBuilder(key, type);
        return _keyBuilder;
    }

    public ModelProfile ToCollection(string name)
    {
        CollectionName = name;
        return this;
    }

    public ModelProfile AutoMap(bool value)
    {
        AutoMapProperty = value;
        return this;
    }

    public ModelProfile BypassValidation(bool value)
    {
        BypassDocumentValidation = value;
        return this;
    }
}

public class ModelProfile<TModel> : ModelProfile
{
    private readonly Dictionary<string, ModelPropertyBuilder> _properties = new();

    internal IReadOnlyDictionary<string, ModelPropertyBuilder> Properties => _properties;

    internal Action<BsonClassMap<TModel>> MapAction { get; private set; }

    public ModelKeyBuilder HasKey(Expression<Func<TModel, object>> selector)
    {
        var property = Reflect.GetProperty(selector);
        return HasKey(property.Name, property.PropertyType);
    }

    public ModelPropertyBuilder HasProperty(Expression<Func<TModel, object>> selector)
    {
        var property = Reflect.GetProperty(selector);
        var builder = new ModelPropertyBuilder(property.Name, property.PropertyType);
        _properties.Add(property.Name, builder);
        return builder;
    }

    public void Map(Action<BsonClassMap<TModel>> map)
    {
        MapAction = map;
    }
}