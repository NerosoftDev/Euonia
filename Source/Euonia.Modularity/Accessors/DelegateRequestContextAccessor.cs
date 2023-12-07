namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// An implementation of <see cref="IRequestContextAccessor"/> using delegate methods.
/// </summary>
public class DelegateRequestContextAccessor : IRequestContextAccessor
{
	private readonly IServiceProvider _provider;
	private readonly RequestContextAccessor _accessor;

	/// <summary>
	/// Initializes a new instance of the <see cref="DelegateRequestContextAccessor"/> class.
	/// </summary>
	public DelegateRequestContextAccessor()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DelegateRequestContextAccessor"/> class.
	/// </summary>
	/// <param name="accessor"></param>
	/// <param name="provider"></param>
	public DelegateRequestContextAccessor(RequestContextAccessor accessor, IServiceProvider provider)
	{
		_accessor = accessor;
		_provider = provider;
	}

	/// <inheritdoc/>
	public RequestContext Context => _accessor?.Invoke(_provider);
}