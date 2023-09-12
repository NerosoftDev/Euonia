namespace Nerosoft.Euonia.Business;

/// <summary>
/// The command object.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CommandObject<T> : BusinessObject<T>, ICommandObject
    where T : CommandObject<T>
{
    /// <summary>
    /// Execute the command.
    /// </summary>
    protected internal virtual async Task ExecuteAsync()
    {
        await Task.CompletedTask;
    }
}