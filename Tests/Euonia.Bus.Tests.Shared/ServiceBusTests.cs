using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Bus.Tests.Commands;

namespace Nerosoft.Euonia.Bus.Tests;

public class ServiceBusTests
{
	private readonly IServiceProvider _provider;
	private readonly bool _preventRunTests;

	public ServiceBusTests(IServiceProvider provider, IConfiguration configuration)
	{
		_provider = provider;
		_preventRunTests = configuration.GetValue<bool>("PreventRunTests");
	}

	[Fact]
	public async Task TestSendCommand_HasResponse()
	{
		if (_preventRunTests)
		{
			Assert.True(true);
		}
		else
		{
			var result = await _provider.GetService<IBus>().SendAsync<UserCreateCommand, int>(new UserCreateCommand());
			Assert.Equal(1, result);
		}
	}

	// [Fact]
	// public async Task TestSendCommand_NoResponse()
	// {
	// 	if (_preventRunTests)
	// 	{
	// 		Assert.True(true);
	// 	}
	// 	else
	// 	{
	// 		await _provider.GetService<IBus>().SendAsync(new UserCreateCommand());
	// 		Assert.True(true);
	// 	}
	// }

	[Fact]
	public async Task TestSendCommand_HasResponse_UseSubscribeAttribute()
	{
		if (_preventRunTests)
		{
			Assert.True(true);
		}
		else
		{
			var result = await _provider.GetService<IBus>().SendAsync(new FooCreateCommand(), new SendOptions { Channel = "foo.create" }, null, (int i) => Console.Write(i));
			Debug.WriteLine(result);
			Assert.Equal(1, result);
		}
	}

	[Fact]
	public async Task TestSendCommand_HasResponse_MessageHasResultInherits()
	{
		if (_preventRunTests)
		{
			Assert.True(true);
		}
		else
		{
			var result = await _provider.GetService<IBus>().RequestAsync<int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
			Assert.Equal(1, result);
		}
	}

	[Fact]
	public async Task TestSendCommand_HasResponse_MessageHasResultInherits_NoRecipient()
	{
		if (_preventRunTests)
		{
			Assert.True(true);
		}
		else
		{
			await Assert.ThrowsAnyAsync<MessageDeliverException>(async () =>
			{
				var _ = await _provider.GetService<IBus>().RequestAsync<int>(new FooCreateCommand());
			});
		}
	}

	[Fact]
	public async Task TestSendCommand_HasResponse_MessageHasResultInherits_ThrowExceptionInHandler()
	{
		if (_preventRunTests)
		{
			Assert.True(true);
		}
		else
		{
			await Assert.ThrowsAnyAsync<NotFoundException>(async () =>
			{
				await _provider.GetService<IBus>().SendAsync(new FooDeleteCommand());
			});
		}
	}
}