using Microsoft.Extensions.Configuration;
using Nerosoft.Euonia.Bus.Tests.Commands;

namespace Nerosoft.Euonia.Bus.Tests;

public partial class ServiceBusTests
{
	private readonly IBus _bus;
	private readonly bool _dontRunTests;

	public ServiceBusTests(IBus bus, IConfiguration configuration)
	{
		_bus = bus;
		_dontRunTests = configuration.GetValue<bool>("DontRunTests");
	}

	public partial async Task TestSendCommand_HasReponse()
	{
		var result = await _bus.SendAsync<UserCreateCommand, int>(new UserCreateCommand());
		Assert.Equal(1, result);
	}

	public partial async Task TestSendCommand_NoReponse()
	{
		await _bus.SendAsync(new UserCreateCommand());
		Assert.True(true);
	}

	public partial async Task TestSendCommand_HasReponse_UseSubscribeAttribute()
	{
		var result = await _bus.SendAsync<FooCreateCommand, int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
		Assert.Equal(1, result);
	}

	public partial async Task TestSendCommand_HasReponse_MessageHasResultInherites()
	{
		var result = await _bus.SendAsync<int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
		Assert.Equal(1, result);
	}
}