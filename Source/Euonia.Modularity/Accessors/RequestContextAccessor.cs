
using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// An implementation of <see cref="IRequestContextAccessor"/>.
/// </summary>
public class RequestContextAccessor : IRequestContextAccessor
{
	private readonly DefaultRequestContextAccessor _defaultAccessor;
	private readonly DelegateRequestContextAccessor _delegateAccessor;
	private readonly RequestContextAccessorOptions _options;

	/// <summary>
	/// Initializes a new instance of the <see cref="RequestContextAccessor"/> class.
	/// </summary>
	/// <param name="options"></param>
	/// <param name="defaultAccessor"></param>
	/// <param name="delegateAccessor"></param>
	public RequestContextAccessor(IOptions<RequestContextAccessorOptions> options, DefaultRequestContextAccessor defaultAccessor, DelegateRequestContextAccessor delegateAccessor)
	{
		_options = options.Value;
		_defaultAccessor = defaultAccessor;
		_delegateAccessor = delegateAccessor;
	}

	/// <summary>
	/// Gets the current request context instance.
	/// </summary>
	public RequestContext Context
	{
		get
		{
			if(_options.UseDefaultAccessor)
			{
				return _defaultAccessor?.Context;
			}
			else
			{
				return _delegateAccessor();
			}
		}
	}
}