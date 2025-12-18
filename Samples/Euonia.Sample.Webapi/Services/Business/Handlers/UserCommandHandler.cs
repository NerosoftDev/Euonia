using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Business;
using Nerosoft.Euonia.Repository;
using Nerosoft.Euonia.Sample.Business.Actuators;
using Nerosoft.Euonia.Sample.Domain.Commands;

namespace Nerosoft.Euonia.Sample.Business.Handlers;

internal sealed class UserCommandHandler(IUnitOfWorkManager unitOfWork, IObjectFactory factory)
	: CommandHandlerBase(unitOfWork, factory),
	  IHandler<UserCreateCommand>
{
	public Task HandleAsync(UserCreateCommand message, MessageContext context, CancellationToken cancellationToken = default)
	{
		return ExecuteAsync(async () =>
		{
			var business = await Factory.CreateAsync<UserGeneralBusiness>(cancellationToken);
			business.Username = message.Username;
			business.Password = message.Password;
			business.Nickname = message.Nickname;
			business.Email = message.Email;
			business.Phone = message.Phone;
			business.MarkAsInsert();
			await business.SaveAsync(false, cancellationToken);
			return business.Id;
		}, context.Response);
	}
}
