namespace Nerosoft.Euonia.Pipeline;

public interface IDelegateBehavior<TRequest>
{
    Task HandleAsync(TRequest request, Delegate next, CancellationToken cancellationToken);
}