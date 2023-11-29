using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nerosoft.Euonia.Bus.Tests.Commands;

namespace Nerosoft.Euonia.Bus.Tests.Handlers
{
	public class UserCommandHandler : IHandler<UserCreateCommand>, IHandler<UserUpdateCommand>
	{
		public bool CanHandle(Type messageType)
		{
			return true;
		}

		public async Task HandleAsync(UserCreateCommand message, MessageContext messageContext, CancellationToken cancellationToken = default)
		{
			Console.WriteLine("UserCreateCommand handled");
			messageContext.Response(1);
			await Task.CompletedTask;
		}

		public async Task HandleAsync(UserUpdateCommand message, MessageContext messageContext, CancellationToken cancellationToken = default)
		{
			Console.WriteLine("UserUpdateCommand handled");
			await Task.CompletedTask;
		}
	}
}
