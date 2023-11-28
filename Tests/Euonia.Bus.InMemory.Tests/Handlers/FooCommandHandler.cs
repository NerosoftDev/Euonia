using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nerosoft.Euonia.Bus.InMemory.Tests.Commands;

namespace Nerosoft.Euonia.Bus.InMemory.Tests.Handlers;

public class FooCommandHandler
{
	[Subscribe("foo.create")]
	public async Task HandleAsync(FooCreateCommand message, MessageContext messageContext, CancellationToken cancellationToken = default)
	{
		Console.WriteLine("FooCreateCommand handled");
		messageContext.Response(1);
		await Task.CompletedTask;
	}
}
