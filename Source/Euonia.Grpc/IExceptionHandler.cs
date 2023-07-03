using Grpc.Core;

namespace Nerosoft.Euonia.Grpc;

public interface IExceptionHandler
{
    /// <summary>
    /// Handle origin exception and generate a new <see cref="RpcException"/>.
    /// </summary>
    /// <param name="exception">The origin exception.</param>
    /// <returns></returns>
    RpcException Handle(Exception exception);
}
