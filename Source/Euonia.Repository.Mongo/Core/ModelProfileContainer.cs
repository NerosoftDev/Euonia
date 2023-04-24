namespace Nerosoft.Euonia.Repository.Mongo;

internal class ModelProfileContainer
{
    private readonly ModelBuilder _builder = new();
    private readonly Dictionary<Type, ModelProfile> _mappings = new();

    private ModelProfileContainer(Action<ModelBuilder> onModelCreating)
    {
        _builder.OnConfigure = OnModelConfigure;
        onModelCreating(_builder);
    }

    public static ModelProfileContainer GetInstance(Action<ModelBuilder> onModelCreating)
    {
        return Singleton<ModelProfileContainer>.Get(() => new ModelProfileContainer(onModelCreating));
    }

    public IReadOnlyDictionary<Type, ModelProfile> Mappings => _mappings;

    private void OnModelConfigure(Type type, ModelProfile profile)
    {
        _mappings[type] = profile;
    }

    internal ModelProfile GetProfile<T>()
    {
        return GetProfile(typeof(T));
    }

    internal ModelProfile GetProfile(Type type)
    {
        return _mappings.TryGetValue(type, out var profile) ? profile : new ModelProfile();
    }
}