namespace Nerosoft.Euonia.Bus.Tests.Commands;

[DispatchIn("inmemory"), ReceiveIn("inmemory")]
[Request(typeof(int))]
public class UserCreateCommand
{
}