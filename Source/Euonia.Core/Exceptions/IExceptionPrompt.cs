namespace Nerosoft.Euonia;

public interface IExceptionPrompt
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    string GetPrompt(Exception exception);
}
