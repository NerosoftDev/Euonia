using Nerosoft.Euonia.Business;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Sample.Business.Rules;
using Nerosoft.Euonia.Sample.Domain.Aggregates;
using Nerosoft.Euonia.Sample.Domain.Repositories;

namespace Nerosoft.Euonia.Sample.Business.Actuators;

internal partial class UserGeneralBusiness(IServiceProvider provider)
	: EditableObjectBase<UserGeneralBusiness>, IDomainService
{
	private IUserRepository _repository;
	private IUserRepository Repository => _repository ??= provider.GetService<IUserRepository>();

	private User Aggregate { get; set; }

	public static readonly PropertyInfo<string> IdProperty = RegisterProperty<string>(p => p.Id);
	public static readonly PropertyInfo<string> UsernameProperty = RegisterProperty<string>(p => p.Username);
	public static readonly PropertyInfo<string> PasswordProperty = RegisterProperty<string>(p => p.Password);
	public static readonly PropertyInfo<string> NicknameProperty = RegisterProperty<string>(p => p.Nickname);
	public static readonly PropertyInfo<string> EmailProperty = RegisterProperty<string>(p => p.Email);
	public static readonly PropertyInfo<string> PhoneProperty = RegisterProperty<string>(p => p.Phone);

	public string Id
	{
		get => GetProperty(IdProperty);
		private set => LoadProperty(IdProperty, value);
	}

	public string Username
	{
		get => GetProperty(UsernameProperty);
		set => SetProperty(UsernameProperty, value);
	}

	public string Password
	{
		get => GetProperty(PasswordProperty);
		set => SetProperty(PasswordProperty, value);
	}

	public string Nickname
	{
		get => GetProperty(NicknameProperty);
		set => SetProperty(NicknameProperty, value);
	}

	public string Email
	{
		get => GetProperty(EmailProperty);
		set => SetProperty(EmailProperty, value);
	}

	public string Phone
	{
		get => GetProperty(PhoneProperty);
		set => SetProperty(PhoneProperty, value);
	}

	protected override void AddRules()
	{
		Rules.AddRule<UsernameAvailabilityCheckRule>(provider);
		Rules.AddRule<EmailAvailabilityCheckRule>(provider);
		Rules.AddRule<PhoneAvailabilityCheckRule>(provider);
		Rules.AddRule(new PasswordStrengthRule());
	}

	[FactoryCreate]
	protected override Task CreateAsync(CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}

	[FactoryFetch]
	protected async Task FetchAsync(string id, CancellationToken cancellationToken = default)
	{
		var aggregate = await Repository.GetAsync(id, true, cancellationToken);

		Aggregate = aggregate ?? throw new NotFoundException();

		using (BypassRuleChecks)
		{
			Id = aggregate.Id;
			Username = aggregate.Username;
			Nickname = aggregate.Nickname;
			Email = aggregate.Email;
			Phone = aggregate.Phone;
		}
	}

	[FactoryInsert]
	protected override Task InsertAsync(CancellationToken cancellationToken = default)
	{
		var user = User.Create(Username, Password);
		if (!string.IsNullOrWhiteSpace(Email))
		{
			user.SetEmail(Email);
		}

		if (!string.IsNullOrWhiteSpace(Phone))
		{
			user.SetPhone(Phone);
		}

		user.SetNickname(Nickname ?? Username);

		return Repository.InsertAsync(user, true, cancellationToken)
						 .ContinueWith(task =>
						 {
							 task.WaitAndUnwrapException(cancellationToken);
							 Id = task.Result.Id;
						 }, cancellationToken);
	}

	[FactoryUpdate]
	protected override Task UpdateAsync(CancellationToken cancellationToken = default)
	{
		if (!HasChangedProperties)
		{
			return Task.CompletedTask;
		}

		if (ChangedProperties.Contains(EmailProperty))
		{
			Aggregate.SetEmail(Email);
		}

		if (ChangedProperties.Contains(PhoneProperty))
		{
			Aggregate.SetPhone(Phone);
		}

		if (ChangedProperties.Contains(NicknameProperty))
		{
			Aggregate.SetNickname(Nickname);
		}

		return _repository.UpdateAsync(Aggregate, true, cancellationToken);
	}
}