﻿using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Claims;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// A pipeline behavior that adds the user principal to the command metadata.
/// </summary>
public class UserPrincipalBehavior : IPipelineBehavior<RoutedMessage<object>, CommandResponse>
{
    private readonly UserPrincipal _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPrincipalBehavior"/> class.
    /// </summary>
    public UserPrincipalBehavior()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPrincipalBehavior"/> class.
    /// </summary>
    /// <param name="user"></param>
    public UserPrincipalBehavior(UserPrincipal user)
    {
        _user = user;
    }

    /// <inheritdoc />
    public async Task<CommandResponse> HandleAsync(RoutedMessage<object> context, PipelineDelegate<RoutedMessage<object>, CommandResponse> next)
    {
        if (_user is { IsAuthenticated: true })
        {
            context.Metadata.Set("$nerosoft:user.name", _user.Username);
            context.Metadata.Set("$nerosoft:user.id", _user.UserId);
            context.Metadata.Set("$nerosoft:user.code", _user.Code);
            context.Metadata.Set("$nerosoft:user.tenant", _user.Tenant);
        }

        return await next(context);
    }
}