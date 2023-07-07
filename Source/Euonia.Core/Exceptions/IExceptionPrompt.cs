namespace Nerosoft.Euonia.Core;

/// <summary>
/// To be added.
/// </summary>
public interface IExceptionPrompt
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    string GetPrompt(Exception exception);
}
