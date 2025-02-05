using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Nerosoft.Euonia.Claims;
using Serilog;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds an IClaimsTransformation for transforming scope claims to the DI container.
	/// <remarks>See https://github.com/IdentityModel/IdentityModel.AspNetCore.AccessTokenValidation/blob/main/src/ScopeClaimsTransformer.cs</remarks>
	/// </summary>
	public static IServiceCollection AddScopeTransformation(this IServiceCollection services)
	{
		return services.AddSingleton<IClaimsTransformation, ScopeClaimsTransformer>();
	}

	/// <summary>
	/// Adds Jwt authentication to the DI container.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="optionsAction"></param>
	/// <returns></returns>
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, Action<JwtAuthenticationOptions> optionsAction)
	{
		var bearerOptions = new JwtAuthenticationOptions();
		optionsAction?.Invoke(bearerOptions);

		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

		var key = Encoding.UTF8.GetBytes(bearerOptions.SigningKey);

		if (bearerOptions.UsePolicy)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
				{
					policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
					policy.RequireClaim(JwtClaimTypes.Subject);
					policy.RequireClaim(JwtClaimTypes.Name);
				});
			});
		}

		services.AddAuthentication(options =>
		        {
			        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		        })
		        .AddJwtBearer(options =>
		        {
			        options.Authority = bearerOptions.Authority;
			        options.RequireHttpsMetadata = bearerOptions.RequireHttpsMetadata;
			        options.Audience = bearerOptions.Audience;

			        options.Events = new JwtBearerEvents()
			        {
				        OnMessageReceived = context =>
				        {
					        var authorizationValue = context.Request.Headers[HeaderNames.Authorization].ToString();

					        var token = Regex.Match(authorizationValue, @"^Bearer\s+(.*)").Groups[1].Value;

					        context.Token = token;
					        return Task.CompletedTask;
				        },
				        OnChallenge = context =>
				        {
					        Console.WriteLine(context);
					        return Task.CompletedTask;
				        },
				        OnAuthenticationFailed = context =>
				        {
					        Console.WriteLine(context);
					        return Task.CompletedTask;
				        },
				        OnForbidden = context =>
				        {
					        Console.WriteLine(context);
					        return Task.CompletedTask;
				        },
				        OnTokenValidated = context =>
				        {
					        Console.WriteLine(context);
					        return Task.CompletedTask;
				        }
			        };
			        options.TokenValidationParameters = new TokenValidationParameters
			        {
				        NameClaimType = JwtClaimTypes.Name,
				        RoleClaimType = JwtClaimTypes.Role,
				        ValidIssuers = bearerOptions.Issuer,
				        //ValidAudience = "api",
				        ValidateIssuer = false,
				        ValidateAudience = false,
				        IssuerSigningKey = new SymmetricSecurityKey(key)
			        };
		        });
		return services;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configurationSectionName"></param>
	/// <returns></returns>
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string configurationSectionName)
	{
		var bearerOptions = services.GetConfiguration().GetSection(configurationSectionName).Get<JwtAuthenticationOptions>();
		return services.AddJwtAuthentication(options =>
		{
			options.Audience = bearerOptions.Audience;
			options.Authority = bearerOptions.Authority;
			options.Issuer = bearerOptions.Issuer;
			options.RequireHttpsMetadata = bearerOptions.RequireHttpsMetadata;
			options.SigningKey = bearerOptions.SigningKey;
		});
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
	{
		var configuration = services.GetSingletonInstance<IConfigureOptions<JwtAuthenticationOptions>>();
		return services.AddJwtAuthentication(configuration.Configure);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IServiceCollection AddUserPrincipal(this IServiceCollection services)
	{
		services.AddHttpContextAccessor();
		services.TryAddScoped(provider =>
		{
			var accessor = provider.GetService<IHttpContextAccessor>();
			return new UserPrincipal(accessor?.HttpContext?.User);
		});
		return services;
	}

	/// <summary>
	/// Add <see cref="Serilog.ILogger"/> as logging provider.
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IServiceCollection AddSerilog(this IServiceCollection services)
	{
		var configuration = services.GetConfiguration();

		services.AddLogging(builder =>
		{
			builder.AddConfiguration(configuration.GetSection("Logging"));
			builder.AddDebug()
			       .AddConsole()
			       .AddSerilog();
		});
		return services;
	}

	/// <summary>
	/// Get hosting environment from <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IWebHostEnvironment GetHostingEnvironment(this IServiceCollection services)
	{
		return services.GetSingletonInstance<IWebHostEnvironment>();
	}
}