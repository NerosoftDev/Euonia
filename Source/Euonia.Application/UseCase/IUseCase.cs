namespace Nerosoft.Euonia.Application;

/// <summary>
/// Defines an interface for use case.
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IUseCase<in TInput, TOutput>
	where TInput : IUseCaseInput
	where TOutput : IUseCaseOutput
{
	/// <summary>
	/// Executes the use case.
	/// </summary>
	/// <param name="input"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the presenter of the use case.
	/// </summary>
	IUseCasePresenter<TOutput> Presenter { get; }
}

/// <summary>
/// Defines an interface for use case.
/// </summary>
/// <typeparam name="TInput"></typeparam>
public interface IUseCase<in TInput>
	where TInput : IUseCaseInput
{
	/// <summary>
	/// Executes the use case.
	/// </summary>
	/// <param name="input"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}