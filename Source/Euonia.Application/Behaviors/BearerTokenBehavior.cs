using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Modularity;
using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// A pipeline behavior that adds the bearer token to the command metadata.
/// </summary>
public class BearerTokenBehavior : IPipelineBehavior<IRoutedMessage, CommandResponse>
{
	private readonly IRequestContextAccessor _contextAccessor;

	/// <summary>
	/// Initializes a new instance of the <see cref="BearerTokenBehavior"/> class.
	/// </summary>
	public BearerTokenBehavior()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="BearerTokenBehavior"/> class.
	/// </summary>
	/// <param name="contextAccessor"></param>
	public BearerTokenBehavior(IRequestContextAccessor contextAccessor)
	{
		_contextAccessor = contextAccessor;
	}

	/// <inheritdoc />
	public async Task<CommandResponse> HandleAsync(IRoutedMessage context, PipelineDelegate<IRoutedMessage, CommandResponse> next)
	{
		if (_contextAccessor?.Context?.RequestHeaders.TryGetValue("Authorization", out var value) == true)
		{
			if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("Bearer") && !value.Equals("Bearer null", StringComparison.OrdinalIgnoreCase))
			{
				context.Metadata.Set("$nerosoft:token", (object)value);
			}
		}

		{
		}

		return await next(context);
	}
}