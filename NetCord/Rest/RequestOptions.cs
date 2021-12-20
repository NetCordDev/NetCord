using System.Net.Http.Headers;

namespace NetCord;
public class RequestOptions
{
    public string? AuditLogReason { get; init; }
    public RetryMode RetryMode { get; init; }

    internal void AddHeaders(HttpRequestHeaders headers)
    {
        if (AuditLogReason != null)
            headers.Add("X-Audit-Log-Reason", AuditLogReason);
    }
}