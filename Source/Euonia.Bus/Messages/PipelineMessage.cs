using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class PipelineMessage<TMessage, TResponse>
	where TMessage : class
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="command"></param>
	public PipelineMessage(TMessage command)
	{
		Command = command;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="command"></param>
	/// <param name="pipeline"></param>
	public PipelineMessage(TMessage command, IPipeline<TMessage, TResponse> pipeline)
	{
		Command = command;
		Pipeline = pipeline;
	}

	/// <summary>
	/// 
	/// </summary>
	public TMessage Command { get; }

	/// <summary>
	/// 
	/// </summary>
	public IPipeline<TMessage, TResponse> Pipeline { get; private set; }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="args"></param>
	/// <returns></returns>
	public PipelineMessage<TMessage, TResponse> Use(Type type, params object[] args)
	{
		Pipeline = Pipeline.Use(type, args);
		return this;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TBehavior"></typeparam>
	/// <returns></returns>
	public PipelineMessage<TMessage, TResponse> Use<TBehavior>()
		where TBehavior : IPipelineBehavior<TMessage, TResponse>
	{
		Pipeline = Pipeline.Use<TBehavior>();
		return this;
	}
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public class PipelineMessage<TMessage>
	where TMessage : class
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="command"></param>
	public PipelineMessage(TMessage command)
	{
		Command = command;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="command"></param>
	/// <param name="pipeline"></param>
	public PipelineMessage(TMessage command, IPipeline pipeline)
	{
		Command = command;
		Pipeline = pipeline;
	}

	/// <summary>
	/// 
	/// </summary>
	public TMessage Command { get; }

	/// <summary>
	/// 
	/// </summary>
	public IPipeline Pipeline { get; private set; }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="args"></param>
	/// <returns></returns>
	public PipelineMessage<TMessage> Use(Type type, params object[] args)
	{
		Pipeline = Pipeline.Use(type, args);
		return this;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TBehavior"></typeparam>
	/// <returns></returns>
	public PipelineMessage<TMessage> Use<TBehavior>()
		where TBehavior : IPipelineBehavior<TMessage>
	{
		Pipeline = Pipeline.Use<TBehavior>();
		return this;
	}
}