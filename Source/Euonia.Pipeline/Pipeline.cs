namespace Nerosoft.Euonia.Pipeline;

public class Pipeline
{
    public static async Task<TResponse> RunAsync<TRequest, TResponse>(TRequest context, Func<TRequest, Task<TResponse>> handler, IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors, CancellationToken cancellationToken = default)
    {
        return await Task.Run(async () =>
        {
            Task<TResponse> accumulate(TRequest _) => handler(context);
            var response = behaviors.Aggregate((PipelineDelegate<TRequest, TResponse>)accumulate, (@delegate, behavior) => request => behavior.HandleAsync(request, @delegate));
            return await response(context);
        }, cancellationToken);
    }

    public static async Task RunAsync<TRequest>(TRequest context, Func<TRequest, Task> handler, IEnumerable<IPipelineBehavior<TRequest>> behaviors, CancellationToken cancellationToken = default)
    {
        await Task.Run(async () =>
        {
            Task accumulate(TRequest _) => handler(context);
            var response = behaviors.Aggregate((PipelineDelegate<TRequest>)accumulate, (@delegate, behavior) => request => behavior.HandleAsync(request, @delegate));
            await response(context);
        }, cancellationToken);
    }

    public static TResponse Run<TRequest, TResponse>(TRequest context, Action<TRequest> accumulate, IEnumerable<IDelegateBehavior<TRequest>> behaviors)
    {
        var response = behaviors.Aggregate((Delegate)accumulate, (@delegate, behavior) => () => behavior.HandleAsync(context, @delegate, default));
        return (TResponse)response.DynamicInvoke(context);
    }

    public static void Run<TRequest>(TRequest context, Action<TRequest> accumulate, IEnumerable<IDelegateBehavior<TRequest>> behaviors)
    {
        var response = behaviors.Aggregate((Delegate)accumulate, (@delegate, behavior) => () => behavior.HandleAsync(context, @delegate, default));
        response.DynamicInvoke(context);
    }
}