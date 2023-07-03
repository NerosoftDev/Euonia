namespace Nerosoft.Euonia.Pipeline;

/// <summary>
/// Defines a pipeline contract.
/// </summary>
public interface IPipeline
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    IPipeline Use(Func<PipelineDelegate, PipelineDelegate> component);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="component"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    IPipeline Use(Func<PipelineDelegate, PipelineDelegate> component, int index);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    IPipeline Use(Func<object, PipelineDelegate, Task> handler);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IPipeline Use(Type type, params object[] args);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBehavior"></typeparam>
    /// <returns></returns>
    IPipeline Use<TBehavior>();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="useAheadOfOthers"></param>
    /// <returns></returns>
    IPipeline UseOf<TContext>(bool useAheadOfOthers = false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contextType"></param>
    /// <param name="useAheadOfOthers"></param>
    /// <returns></returns>
    IPipeline UseOf(Type contextType, bool useAheadOfOthers = false);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    PipelineDelegate Build();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task RunAsync(object context);

    Task RunAsync(object context, Func<object, Task> accumulate);
}

public interface IPipeline<TRequest, TResponse>
{
    IPipeline<TRequest, TResponse> Use(Func<PipelineDelegate<TRequest, TResponse>, PipelineDelegate<TRequest, TResponse>> component);

    IPipeline<TRequest, TResponse> Use(Func<PipelineDelegate<TRequest, TResponse>, PipelineDelegate<TRequest, TResponse>> component, int index);

    //IPipeline<TRequest, TResponse> Use(Func<TRequest, Func<Task<TResponse>>, Task<TResponse>> handler);

    IPipeline<TRequest, TResponse> Use(Func<TRequest, PipelineDelegate<TRequest, TResponse>, Task<TResponse>> handler);

    IPipeline<TRequest, TResponse> Use(Type type, params object[] args);

    IPipeline<TRequest, TResponse> Use<TBehavior>() where TBehavior : IPipelineBehavior<TRequest, TResponse>;

    IPipeline<TRequest, TResponse> UseOf<TContext>(bool useAheadOfOthers = false);

    IPipeline<TRequest, TResponse> UseOf(Type contextType, bool useAheadOfOthers = false);

    PipelineDelegate<TRequest, TResponse> Build();

    Task<TResponse> RunAsync(TRequest context);

    Task<TResponse> RunAsync(TRequest context, Func<TRequest, Task<TResponse>> accumulate);
}