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
    /// Build pipeline delegate.
    /// </summary>
    /// <returns></returns>
    PipelineDelegate Build();

    /// <summary>
    /// Run pipeline delegate.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task RunAsync(object context);

    /// <summary>
    /// Run pipeline delegate.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="accumulate"></param>
    /// <returns></returns>
    Task RunAsync(object context, Func<object, Task> accumulate);
}

/// <summary>
/// Defines a pipeline contract.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IPipeline<TRequest, TResponse>
{
	/// <summary>
	/// Use pipeline delegate component.
	/// </summary>
	/// <param name="component"></param>
	/// <returns></returns>
    IPipeline<TRequest, TResponse> Use(Func<PipelineDelegate<TRequest, TResponse>, PipelineDelegate<TRequest, TResponse>> component);

	/// <summary>
	/// Use pipeline delegate component.
	/// </summary>
	/// <param name="component"></param>
	/// <param name="index"></param>
	/// <returns></returns>
    IPipeline<TRequest, TResponse> Use(Func<PipelineDelegate<TRequest, TResponse>, PipelineDelegate<TRequest, TResponse>> component, int index);

    //IPipeline<TRequest, TResponse> Use(Func<TRequest, Func<Task<TResponse>>, Task<TResponse>> handler);

    /// <summary>
    /// Use pipeline delegate component.
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    IPipeline<TRequest, TResponse> Use(Func<TRequest, PipelineDelegate<TRequest, TResponse>, Task<TResponse>> handler);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IPipeline<TRequest, TResponse> Use(Type type, params object[] args);

    /// <summary>
    /// Use pipeline behavior.
    /// </summary>
    /// <typeparam name="TBehavior"></typeparam>
    /// <returns></returns>
    IPipeline<TRequest, TResponse> Use<TBehavior>() where TBehavior : IPipelineBehavior<TRequest, TResponse>;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="useAheadOfOthers"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    IPipeline<TRequest, TResponse> UseOf<TContext>(bool useAheadOfOthers = false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contextType"></param>
    /// <param name="useAheadOfOthers"></param>
    /// <returns></returns>
    IPipeline<TRequest, TResponse> UseOf(Type contextType, bool useAheadOfOthers = false);

    /// <summary>
    /// Build pipeline delegate.
    /// </summary>
    /// <returns></returns>
    PipelineDelegate<TRequest, TResponse> Build();

    /// <summary>
    /// Run pipeline delegate.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<TResponse> RunAsync(TRequest context);

    /// <summary>
    /// Run pipeline delegate.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="accumulate"></param>
    /// <returns></returns>
    Task<TResponse> RunAsync(TRequest context, Func<TRequest, Task<TResponse>> accumulate);
}