namespace System;

/// <summary>
/// Responsible for storing and returning exception prompts.
/// </summary>
public static class ExceptionPrompt
{
    private static readonly List<IExceptionPrompt> _prompts = new();

    /// <summary>
    /// Add a prompt to the list of prompts.
    /// </summary>
    /// <param name="prompt"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddPrompt(IExceptionPrompt prompt)
    {
        if (prompt == null)
        {
            throw new ArgumentNullException(nameof(prompt));
        }

        if (_prompts.Contains(prompt))
        {
            return;
        }

        _prompts.Add(prompt);
    }

    /// <summary>
    /// Gets the prompt for the specified exception.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string GetPrompt(Exception exception)
    {
        exception = exception.GetBaseException();
        var prompt = GetExceptionPrompt(exception);
        if (string.IsNullOrWhiteSpace(prompt) == false)
        {
            return prompt;
        }

        if (exception is ApplicationException applicationException)
        {
            return applicationException.Message;
        }

        return "Application error";
    }

    private static string GetExceptionPrompt(Exception exception)
    {
        foreach (var prompt in _prompts)
        {
            var result = prompt.GetPrompt(exception);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
        }

        return string.Empty;
    }
}
