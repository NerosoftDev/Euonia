using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Application;

public class PipelineCommand<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    public PipelineCommand(TCommand command)
    {
        Command = command;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="pipeline"></param>
    public PipelineCommand(TCommand command, IPipeline<ICommand, CommandResponse> pipeline)
    {
        Command = command;
        Pipeline = pipeline;
    }

    /// <summary>
    /// 
    /// </summary>
    public TCommand Command { get; }

    /// <summary>
    /// 
    /// </summary>
    public IPipeline<ICommand, CommandResponse> Pipeline { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public PipelineCommand<TCommand> Use(Type type, params object[] args)
    {
        Pipeline = Pipeline.Use(type, args);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBehavior"></typeparam>
    /// <returns></returns>
    public PipelineCommand<TCommand> Use<TBehavior>()
        where TBehavior : IPipelineBehavior<ICommand, CommandResponse>
    {
        Pipeline = Pipeline.Use<TBehavior>();
        return this;
    }
}

/// <summary>
/// 
/// </summary>
public static class PipelineCommandExtensions
{
    public static PipelineCommand<TCommand> UsePipeline<TCommand>(this TCommand command, IPipeline<ICommand, CommandResponse> pipeline)
        where TCommand : ICommand
    {
        return new PipelineCommand<TCommand>(command, pipeline);
    }
}