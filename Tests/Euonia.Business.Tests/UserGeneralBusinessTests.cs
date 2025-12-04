using Nerosoft.Euonia.Business;

namespace Nerosoft.Euonia.Core.Tests;

public class UserGeneralBusinessTests
{
	private IObjectFactory _factory;

	public UserGeneralBusinessTests(IObjectFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task LazyServiceProviderShouldWork()
	{
		var business = await _factory.CreateAsync<UserGeneralBusiness>(CancellationToken.None);
		Assert.NotNull(business.LazyServiceProvider);
	}

	[Fact]
	public async Task CreateShouldFailDueToPermissionDenied()
	{
		var business = await _factory.CreateAsync<UserGeneralBusiness>(CancellationToken.None);
		business.Name = "admin";
		var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
		{
			business.MarkAsInsert();
			await business.SaveAsync(false, CancellationToken.None);
		});
		Assert.Equal(2, ex.Errors.Count());
		//Assert.Equal("PermissionDenied", ex.Errors.FirstOrDefault()!.ErrorMessage);
	}

	[Fact]
	public async Task UpdateShouldNotFailDueToPermissionDenied()
	{
		var business = await _factory.CreateAsync<UserGeneralBusiness>(CancellationToken.None);
		business.Name = "admin";
		business.MarkAsUpdate();
		// var ex = await Record.ExceptionAsync(async () =>
		// {
		// 	await business.SaveAsync(false, CancellationToken.None);
		// });
		var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
		{
			await business.SaveAsync(true, CancellationToken.None);
		});
		Assert.Single(ex.Errors);
	}

	[Fact]
	public async Task DeleteShouldFailDueToPermissionDenied_EnabledCheckObjectRuleOnDelete()
	{
		var business = await _factory.CreateAsync<UserGeneralBusiness>(CancellationToken.None);
		business.Name = "admin";
		business.MarkAsDelete(true);
		// var ex = await Record.ExceptionAsync(async () =>
		// {
		// 	await business.SaveAsync(false, CancellationToken.None);
		// });
		var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
		{
			await business.SaveAsync(false, CancellationToken.None);
		});
		Assert.Equal("PermissionDenied", ex.Errors.FirstOrDefault()!.ErrorMessage);
	}
	
	[Fact]
	public async Task DeleteShouldNotFailDueToPermissionDenied_DisabledCheckObjectRuleOnDelete()
	{
		var business = await _factory.CreateAsync<UserGeneralBusiness>(CancellationToken.None);
		business.Name = "admin";
		business.MarkAsDelete();
		var exception = await Record.ExceptionAsync(async () =>
		{
			await business.SaveAsync(false, CancellationToken.None);
		});
		Assert.Null(exception);
	}
}