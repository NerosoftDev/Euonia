namespace Nerosoft.Euonia.Repository.Mongo;

public class ModelPropertyBuilder
{
    internal ModelPropertyBuilder(string propertyName, Type propertyType)
    {
        PropertyName = propertyName;
        PropertyType = propertyType;
    }

    internal string PropertyName { get; }

    internal Type PropertyType { get; }

    internal string ElementName { get; private set; }

    internal bool? IsRequired { get; private set; }

    internal IValueGenerator ValueGenerator { get; private set; }

    public ModelPropertyBuilder HasElementName(string elementName)
    {
        ElementName = elementName;
        return this;
    }

    public ModelPropertyBuilder Required(bool value)
    {
        IsRequired = value;
        return this;
    }

    public ModelPropertyBuilder HasValueGenerator<TGenerator>()
        where TGenerator : class, IValueGenerator, new()
    {
        ValueGenerator = Singleton<TGenerator>.Get(() => new());
        return this;
    }

    public ModelPropertyBuilder HasValueGenerator(IValueGenerator generator)
    {
        ValueGenerator = generator;
        return this;
    }
}