namespace Nerosoft.Euonia.Bus.Tests;

internal partial class Defines
{
	public const bool DontRunTests =
#if DEBUG
	false
#else
	true
#endif
	;

}
