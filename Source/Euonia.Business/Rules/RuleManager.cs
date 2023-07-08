using System.Collections.Concurrent;

namespace Nerosoft.Euonia.Business;

public class RuleManager
{
    private static readonly Lazy<ConcurrentDictionary<Type, RuleManager>> _container = new();

    private RuleManager()
    {
        Rules = new List<IRuleBase>();
    }

    public bool Initialized { get; set; }

    public List<IRuleBase> Rules { get; }

    public static RuleManager GetRules<T>()
    {
        return GetRules(typeof(T));
    }

    public static RuleManager GetRules(Type type)
    {
        var result = _container.Value.GetOrAdd(type, t => new RuleManager());
        return result;
    }

    public static void CleanRules(Type type)
    {
        lock (_container)
        {
            _container.Value.TryRemove(type, out var _);
        }
    }
}