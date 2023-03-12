namespace Nerosoft.Euonia.Domain;

public class AuditingRecord
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string TenantId { get; set; }

    public string TenantName { get; set; }

    public DateTime ExecutionTime { get; set; }

    public int ExecutionDuration { get; set; }

    public string ClientId { get; set; }

    public string CorrelationId { get; set; }

    public string ClientIpAddress { get; set; }

    public string ClientName { get; set; }

    public string BrowserInfo { get; set; }

    public string HttpMethod { get; set; }

    public int? HttpStatusCode { get; set; }

    public string Url { get; set; }
}