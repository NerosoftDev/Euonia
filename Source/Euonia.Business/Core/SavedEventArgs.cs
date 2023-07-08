namespace Nerosoft.Euonia.Business;

public class SavedEventArgs : EventArgs
{
    public SavedEventArgs(object newObject)
    {
        NewObject = newObject;
    }

    public SavedEventArgs(object newObject, Exception error, object userState)
        : this(newObject)
    {
        Error = error;
        UserState = userState;
    }

    public object NewObject { get; }

    public Exception Error { get; }

    public object UserState { get; }
}