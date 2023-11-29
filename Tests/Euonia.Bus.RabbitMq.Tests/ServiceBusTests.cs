using Microsoft.Extensions.Configuration;
using Nerosoft.Euonia.Bus.Tests.Commands;

namespace Nerosoft.Euonia.Bus.Tests;

#if DEBUG
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
		if (_dontRunTests)
		{
			return;
		}
		var result = await _bus.SendAsync<UserCreateCommand, int>(new UserCreateCommand());
		Assert.Equal(1, result);
	}

	public partial async Task TestSendCommand_NoReponse()
	{
		if (_dontRunTests)
		{
			return;
		}
		await _bus.SendAsync(new UserCreateCommand());
		Assert.True(true);
	}

	public partial async Task TestSendCommand_HasReponse_UseSubscribeAttribute()
	{
		if (_dontRunTests)
		{
			return;
		}
		var result = await _bus.SendAsync<FooCreateCommand, int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
		Assert.Equal(1, result);
	}

	public partial async Task TestSendCommand_HasReponse_MessageHasResultInherites()
	{
		if (_dontRunTests)
		{
			return;
		}
		var result = await _bus.SendAsync<int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
		Assert.Equal(1, result);
	}
}
#else
public partial class ServiceBusTests
{
	public partial async Task TestSendCommand_HasReponse()
	{
		Assert.True(true);
	}

	public partial async Task TestSendCommand_NoReponse()
	{
		Assert.True(true);
	}

	public partial async Task TestSendCommand_HasReponse_UseSubscribeAttribute()
	{
		Assert.True(true);
	}

	public partial async Task TestSendCommand_HasReponse_MessageHasResultInherites()
	{
		Assert.True(true);
	}
}
#endif