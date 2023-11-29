namespace Nerosoft.Euonia.Bus.Tests;

public partial class ServiceBusTests
{
	[Fact]
	public partial Task TestSendCommand_HasReponse();

	[Fact]
	public partial Task TestSendCommand_NoReponse();

	[Fact]
	public partial Task TestSendCommand_HasReponse_UseSubscribeAttribute();

	[Fact]
	public partial Task TestSendCommand_HasReponse_MessageHasResultInherites();
}