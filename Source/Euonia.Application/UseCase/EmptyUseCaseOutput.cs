namespace Nerosoft.Euonia.Application;

/// <summary>
/// Represents the use case should not return any output.
/// </summary>
public sealed record EmptyUseCaseOutput : IUseCaseOutput
{
	/// <summary>
	/// Gets a new instance of <see cref="EmptyUseCaseOutput"/>.
	/// </summary>
	public static EmptyUseCaseOutput Instance { get; } = new();
}