using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// A pipeline behavior that adds the bearer token to the command metadata.
/// </summary>
public class BearerTokenBehavior : IPipelineBehavior<ICommand, CommandResponse>
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
    public async Task<CommandResponse> HandleAsync(ICommand context, PipelineDelegate<ICommand, CommandResponse> next)
    {
        if (_contextAccessor?.RequestHeaders.TryGetValue("Authorization", out var values) == true)
        {
            if (values.Count > 0)
            {
                var value = values[0];
                if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("Bearer") && !value.Equals("Bearer null", StringComparison.OrdinalIgnoreCase))
                {
                    context.Metadata.Set("$nerosoft:token", value);
                }
            }
        }

        {
        }

        return await next(context);
    }
}