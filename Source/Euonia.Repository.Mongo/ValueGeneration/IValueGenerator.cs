namespace Nerosoft.Euonia.Repository.Mongo;

public interface IValueGenerator
{
    object Generate();
}

public interface IValueGenerator<TValue> : IValueGenerator
{
    new TValue Generate();
}