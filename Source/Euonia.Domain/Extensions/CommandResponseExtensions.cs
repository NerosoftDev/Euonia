using Nerosoft.Euonia.Core;

namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Extension methods for <see cref="CommandResponse"/>.
/// </summary>
public static class CommandResponseExtensions
{
    /// <summary>
    /// Set response status to <see cref="CommandStatus.Succeed"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public static CommandResponse Success(this CommandResponse response)
    {
        response.Status = CommandStatus.Succeed;
        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static CommandResponse Failure(this CommandResponse response, Exception exception)
    {
        //response.Status = GetStatus(exception);
        response.Error = exception;
        response.Message = exception.Message;
        response.Code = exception switch
        {
            BusinessException e => e.Code,
            HttpRequestException e => $"HTTP{e.StatusCode}",
            _ => response.Code
        };

        return response;
    }

    /// <summary>
    /// Set response code and return the current instance.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public static CommandResponse WithCode(this CommandResponse response, string code)
    {
        response.Code = code;
        return response;
    }

    /// <summary>
    /// Set response message and return the current instance.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static CommandResponse WithMessage(this CommandResponse response, string message)
    {
        response.Message = message;
        return response;
    }

    /// <summary>
    /// Set response error and return the current instance.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public static CommandResponse WithStatus(this CommandResponse response, CommandStatus status)
    {
        response.Status = status;
        return response;
    }

    /// <summary>
    /// Sets response status to <see cref="CommandStatus.Succeed"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static CommandResponse<TResult> Success<TResult>(this CommandResponse<TResult> response)
    {
        response.Status = CommandStatus.Succeed;
        return response;
    }

    /// <summary>
    /// Set response status to <see cref="CommandStatus.Succeed"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="result"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static CommandResponse<TResult> Success<TResult>(this CommandResponse<TResult> response, TResult result)
    {
        return response.Success().WithResult(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="exception"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static CommandResponse<TResult> Failure<TResult>(this CommandResponse<TResult> response, Exception exception)
    {
        //response.Status = GetStatus(exception);
        response.Error = exception;
        response.Message = exception.Message;
        response.Code = exception switch
        {
            BusinessException e => e.Code,
            HttpRequestException e => $"HTTP{e.StatusCode}",
            _ => response.Code
        };

        return response;
    }

    /// <summary>
    /// Sets response code and return the current instance.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="code"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static CommandResponse<TResult> WithCode<TResult>(this CommandResponse<TResult> response, string code)
    {
        response.Code = code;
        return response;
    }

    /// <summary>
    /// Sets response message and return the current instance.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="message"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static CommandResponse<TResult> WithMessage<TResult>(this CommandResponse<TResult> response, string message)
    {
        response.Message = message;
        return response;
    }

    /// <summary>
    /// Sets response result and return the current instance.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="result"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static CommandResponse<TResult> WithResult<TResult>(this CommandResponse<TResult> response, TResult result)
    {
        response.Result = result;
        return response;
    }

    /// <summary>
    /// Sets the response status and return the current instance.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="status"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static CommandResponse<TResult> WithStatus<TResult>(this CommandResponse<TResult> response, CommandStatus status)
    {
        response.Status = status;
        return response;
    }
}