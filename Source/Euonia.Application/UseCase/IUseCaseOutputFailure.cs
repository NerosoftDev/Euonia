namespace Nerosoft.Euonia.Application;

/// <summary>
/// Represents the structure of use case output when it is failure.
/// </summary>
public interface IUseCaseOutputFailure
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="exception"></param>
	void Error(Exception exception);
}