using Nerosoft.Euonia.Bus.Tests.Commands;

namespace Nerosoft.Euonia.Bus.Tests;

public class ServiceBusTests
{
	private readonly IBus _bus;

	public ServiceBusTests(IBus bus)
	{
		_bus = bus;
	}

	[Fact]
	public async Task TestSendCommand_HasReponse()
	{
		var result = await _bus.SendAsync<UserCreateCommand, int>(new UserCreateCommand());
		Assert.Equal(1, result);
	}

	[Fact]
	public async Task TestSendCommand_NoReponse()
	{
		await _bus.SendAsync(new UserCreateCommand());
		Assert.True(true);
	}

	[Fact]
	public async Task TestSendCommand_HasReponse_UseSubscribeAttribute()
	{
		var result = await _bus.SendAsync<FooCreateCommand, int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
		Assert.Equal(1, result);
	}

	[Fact]
	public async Task TestSendCommand_HasReponse_MessageHasResultInherites()
	{
		var result = await _bus.SendAsync<int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
		Assert.Equal(1, result);
	}
}