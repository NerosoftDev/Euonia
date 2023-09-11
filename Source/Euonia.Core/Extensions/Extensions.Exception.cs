using System.Runtime.ExceptionServices;

public static partial class Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="maxDepths"></param>
    /// <returns></returns>
    public static string GetFullMessage(this Exception exception, int maxDepths = 3)
    {
        if (exception == null)
        {
            return null;
        }

        var message = new StringBuilder();
        while (exception != null && maxDepths > 0)
        {
            message.AppendLine(exception.Message);
            message.AppendLine("====================");
            exception = exception.InnerException;
            maxDepths--;
        }

        return message.ToString();
    }

    /// <summary>
    /// Gets message of the first thrown in a chain of exceptions.
    /// </summary>
    /// <param name="exception">The caught exception.</param>
    /// <returns></returns>
    public static string GetRootMessage(this Exception exception)
    {
        while (true)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            if (exception.InnerException == null)
            {
                return exception.Message;
            }

            exception = exception.InnerException;
        }
    }

    /// <summary>
    /// Attempts to prepare the exception for re-throwing by preserving the stack trace. The returned exception should be immediately thrown.
    /// </summary>
    /// <param name="exception">The exception. May not be <c>null</c>.</param>
    /// <returns>The <see cref="Exception"/> that was passed into this method.</returns>
    public static Exception PrepareForRethrow(this Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();

        // The code cannot ever get here. We just return a value to work around a badly-designed API (ExceptionDispatchInfo.Throw):
        //  https://connect.microsoft.com/VisualStudio/feedback/details/689516/exceptiondispatchinfo-api-modifications (http://www.webcitation.org/6XQ7RoJmO)
        return exception;
    }
}