using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines the default message transportation dispatcher.
/// </summary>
public class DefaultDispatcher : IDispatcher
{
	private readonly IServiceProvider _provider;

	/// <summary>
	/// Initializes a new instance of the <see cref="DefaultDispatcher"/> class.
	/// </summary>
	/// <param name="provider"></param>
	public DefaultDispatcher(IServiceProvider provider)
	{
		_provider = provider;
	}

	/// <inheritdoc />
	public ITransport CreateTransport(Type messageType)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public ITransport CreateTransport(string identifier)
	{
		return _provider.GetKeyedService<ITransport>(identifier);
	}
}