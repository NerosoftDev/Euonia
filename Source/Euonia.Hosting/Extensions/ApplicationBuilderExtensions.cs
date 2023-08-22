﻿using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Nerosoft.Euonia.Dependency;
using Nerosoft.Euonia.Modularity;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// 
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCulture(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var values))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(values);
            }

            await next();
        });
        return app;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Local
    private static IApplicationBuilder UseJwtToken(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var values))
            {
                if (values.Count > 0)
                {
                    var value = values[0];
                    if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("Bearer") && !value.Equals("Bearer null", StringComparison.OrdinalIgnoreCase))
                    {
                        var tokenString = value.Replace("Bearer", string.Empty).Trim();
                        var handler = new JwtSecurityTokenHandler();
                        var token = handler.ReadJwtToken(tokenString);
                        var claims = token.Claims;
                        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "AuthenticationTypes.Federation", "name", "role"));
                    }
                }
            }

            await next();
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseJwt(this IApplicationBuilder app, string schema = JwtBearerDefaults.AuthenticationScheme)
    {
        return app.Use(async (context, next) =>
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                var result = await context.AuthenticateAsync(schema);
                if (result.Succeeded && result.Principal != null)
                {
                    context.User = result.Principal;
                }
            }

            await next();
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public static void InitializeApplication(this IApplicationBuilder app)
    {
        app.ApplicationServices.GetRequiredService<ObjectAccessor<IApplicationBuilder>>().Value = app;
        var application = app.ApplicationServices.GetRequiredService<IApplicationWithServiceProvider>();
        var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        applicationLifetime.ApplicationStopping.Register(() =>
        {
            application.Shutdown();
        });

        applicationLifetime.ApplicationStopped.Register(() =>
        {
            application.Dispose();
        });

        application.Initialize(app.ApplicationServices);
    }
}