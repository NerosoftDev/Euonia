using System.Reflection;
using System.Runtime.Loader;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Nerosoft.Euonia.Reflection;

/// <summary>
/// Exposes static methods for load assembly and types.
/// </summary>
public static class AssemblyHelper
{
    /// <summary>
    /// Load assemblies from executable files found in <paramref name="directory"/>.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <param name="searchOption">The option specifies whether the search operation should include only the current directory or all subdirectories.</param>
    /// <returns>The found assemblies.</returns>
    public static List<Assembly> LoadAssemblies(string directory, SearchOption searchOption)
    {
        return GetAssemblyFiles(directory, searchOption).Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                                                        .ToList();
    }

    /// <summary>
    /// Gets assembly files in <paramref name="directory"/>.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <param name="searchOption">The option specifies whether the search operation should include only the current directory or all subdirectories.</param>
    /// <returns>The found assembly files.</returns>
    public static IEnumerable<string> GetAssemblyFiles(string directory, SearchOption searchOption)
    {
        return Directory.EnumerateFiles(directory, "*.*", searchOption)
                        .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));
    }

    /// <summary>
    /// Gets all types in <paramref name="assembly"/>.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>Types found in assembly.</returns>
    public static IReadOnlyList<Type> GetAllTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types;
        }
    }
}