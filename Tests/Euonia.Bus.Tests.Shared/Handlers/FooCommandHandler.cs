using Nerosoft.Euonia.Bus.Tests.Commands;

namespace Nerosoft.Euonia.Bus.Tests.Handlers;

public class FooCommandHandler
{
	[Subscribe("foo.create")]
	public Task<int> HandleAsync(FooCreateCommand message, MessageContext messageContext, CancellationToken cancellationToken = default)
	{
		Console.WriteLine("FooCreateCommand handled");
		//messageContext.Response(1);
		//await Task.CompletedTask;
		return Task.FromResult(1);
	}

	[Subscribe]
	public async Task HandleAsync(FooDeleteCommand message, MessageContext messageContext, CancellationToken cancellationToken = default)
	{
		throw new NotFoundException();
	}
}