namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The entity of auditing record.
/// </summary>
public class AuditingRecord
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the tenant id.
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the tenant name.
    /// </summary>
    public string TenantName { get; set; }

    /// <summary>
    /// Gets or sets the operation execution time.
    /// </summary>
    public DateTime ExecutionTime { get; set; }

    /// <summary>
    /// Gets or sets the operation execution duration.
    /// </summary>
    public int ExecutionDuration { get; set; }

    /// <summary>
    /// Gets or sets the client id
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the correlation id
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the client ip address
    /// </summary>
    public string ClientIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the client name
    /// </summary>
    public string ClientName { get; set; }

    /// <summary>
    /// Gets or sets the browser info
    /// </summary>
    public string BrowserInfo { get; set; }

    /// <summary>
    /// Gets or sets the http method
    /// </summary>
    public string HttpMethod { get; set; }

    /// <summary>
    /// Gets or sets the response status code
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// Gets or sets the request url
    /// </summary>
    public string Url { get; set; }
}