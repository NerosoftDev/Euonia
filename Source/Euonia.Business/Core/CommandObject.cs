namespace Nerosoft.Euonia.Business;

public abstract class CommandObject<T> : BusinessObject<T>, ICommandObject
    where T : CommandObject<T>
{
    protected internal virtual async Task ExecuteAsync()
    {
        await Task.CompletedTask;
    }
}