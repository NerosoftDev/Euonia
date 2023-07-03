using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// 
/// </summary>
public interface IMethodInvocation
{
    /// <summary>
    /// 
    /// </summary>
    object[] Arguments { get; }

    /// <summary>
    /// 
    /// </summary>
    IReadOnlyDictionary<string, object> ArgumentsDictionary { get; }

    /// <summary>
    /// 
    /// </summary>
    Type[] GenericArguments { get; }

    /// <summary>
    /// 
    /// </summary>
    object TargetObject { get; }

    /// <summary>
    /// 
    /// </summary>
    MethodInfo Method { get; }

    /// <summary>
    /// 
    /// </summary>
    object ReturnValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task ProceedAsync();
}