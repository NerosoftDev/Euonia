namespace Nerosoft.Euonia.Pipeline;

/// <summary>
/// The pipeline delegate.
/// </summary>
public class Pipeline
{
    /// <summary>
    /// Run the pipeline.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="handler"></param>
    /// <param name="behaviors"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public static async Task<TResponse> RunAsync<TRequest, TResponse>(TRequest context, Func<TRequest, Task<TResponse>> handler, IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors, CancellationToken cancellationToken = default)
    {
        return await Task.Run(async () =>
        {
            Task<TResponse> Accumulate(TRequest _) => handler(context);
            var response = behaviors.Aggregate((PipelineDelegate<TRequest, TResponse>)Accumulate, (@delegate, behavior) => request => behavior.HandleAsync(request, @delegate));
            return await response(context);
        }, cancellationToken);
    }

    /// <summary>
    /// Run the pipeline.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="handler"></param>
    /// <param name="behaviors"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TRequest"></typeparam>
    public static async Task RunAsync<TRequest>(TRequest context, Func<TRequest, Task> handler, IEnumerable<IPipelineBehavior<TRequest>> behaviors, CancellationToken cancellationToken = default)
    {
        await Task.Run(async () =>
        {
            Task Accumulate(TRequest _) => handler(context);
            var response = behaviors.Aggregate((PipelineDelegate<TRequest>)Accumulate, (@delegate, behavior) => request => behavior.HandleAsync(request, @delegate));
            await response(context);
        }, cancellationToken);
    }

    /// <summary>
    /// Run the pipeline.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="accumulate"></param>
    /// <param name="behaviors"></param>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public static TResponse Run<TRequest, TResponse>(TRequest context, Action<TRequest> accumulate, IEnumerable<IDelegateBehavior<TRequest>> behaviors)
    {
        var response = behaviors.Aggregate((Delegate)accumulate, (@delegate, behavior) => () => behavior.HandleAsync(context, @delegate, default));
        return (TResponse)response.DynamicInvoke(context);
    }

    /// <summary>
    /// Run the pipeline.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="accumulate"></param>
    /// <param name="behaviors"></param>
    /// <typeparam name="TRequest"></typeparam>
    public static void Run<TRequest>(TRequest context, Action<TRequest> accumulate, IEnumerable<IDelegateBehavior<TRequest>> behaviors)
    {
        var response = behaviors.Aggregate((Delegate)accumulate, (@delegate, behavior) => () => behavior.HandleAsync(context, @delegate, default));
        response.DynamicInvoke(context);
    }
}