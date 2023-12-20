namespace Nerosoft.Euonia.Application;

/// <summary>
/// Represents the structure of use case output when it is success.
/// </summary>
/// <typeparam name="TOutput"></typeparam>
public interface IUseCaseOutputSuccess<in TOutput>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="output"></param>
	void Ok(TOutput output);
}