﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Bus.Tests.Commands;

namespace Nerosoft.Euonia.Bus.Tests;

public partial class ServiceBusTests
{
	private readonly IServiceProvider _provider;
	private readonly bool _preventRunTests;

	public ServiceBusTests(IServiceProvider provider, IConfiguration configuration)
	{
		_provider = provider;
		_preventRunTests = configuration.GetValue<bool>("PreventRunTests");
	}

	[Fact]
	public async Task TestSendCommand_HasReponse()
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

	[Fact]
	public async Task TestSendCommand_NoReponse()
	{
		if (_preventRunTests)
		{
			Assert.True(true);
		}
		else
		{
			await _provider.GetService<IBus>().SendAsync(new UserCreateCommand());
			Assert.True(true);
		}
	}

	[Fact]
	public async Task TestSendCommand_HasReponse_UseSubscribeAttribute()
	{
		if (_preventRunTests)
		{
			Assert.True(true);
		}
		else
		{
			var result = await _provider.GetService<IBus>().SendAsync<FooCreateCommand, int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
			Assert.Equal(1, result);
		}
	}

	[Fact]
	public async Task TestSendCommand_HasReponse_MessageHasResultInherites()
	{
		if (_preventRunTests)
		{
			Assert.True(true);
		}
		else
		{
			var result = await _provider.GetService<IBus>().SendAsync<int>(new FooCreateCommand(), new SendOptions { Channel = "foo.create" });
			Assert.Equal(1, result);
		}
	}
}