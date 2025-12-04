using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Nerosoft.Euonia.Hosting;

internal static class RequestContextExtensions
{
	extension(RequestContext)
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static RequestContext From(HttpContext context)
		{
			return new RequestContext
			{
				RequestHeaders = context.Request?.Headers?.ToDictionary(t => t.Key, t => t.Value.ToString()),
				ConnectionId = context.Connection?.Id,
				User = new ClaimsPrincipal(context.User),
				RemotePort = context.Connection?.RemotePort ?? 0,
				RemoteIpAddress = context.Connection?.RemoteIpAddress,
				RequestAborted = context.RequestAborted,
				IsWebSocketRequest = context.WebSockets?.IsWebSocketRequest ?? false,
				TraceIdentifier = context.TraceIdentifier,
				RequestServices = context.RequestServices
			};
		}
	}
}