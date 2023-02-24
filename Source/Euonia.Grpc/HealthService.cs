using Grpc.Health.V1;
using Grpc.HealthCheck;

namespace Nerosoft.Euonia.Grpc
{
    /// <summary>
    /// 
    /// </summary>
    public class HealthService : HealthServiceImpl
    {
        /// <summary>
        /// 
        /// </summary>
        private const string SERVICE_NAME = "HealthCheck";

        /// <inheritdoc />
        public HealthService()
        {
            SetStatus(SERVICE_NAME, HealthCheckResponse.Types.ServingStatus.Serving);
        }
    }
}