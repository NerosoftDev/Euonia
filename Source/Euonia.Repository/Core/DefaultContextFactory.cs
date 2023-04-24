using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Repository;

/// <summary>
/// 
/// </summary>
public class DefaultContextFactory : IContextFactory
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    public DefaultContextFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public TContext GetContext<TContext>()
        where TContext : class, IRepositoryContext
    {
        return _provider.GetRequiredService<TContext>();
    }

    /// <inheritdoc />
    public int Order => int.MaxValue;
}