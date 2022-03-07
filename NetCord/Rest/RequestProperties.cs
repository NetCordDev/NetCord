using System.Net.Http.Headers;

namespace NetCord;

public class RequestProperties
{
    public string? AuditLogReason { get; set; }
    public RetryMode RetryMode { get; set; }

    internal void AddHeaders(HttpRequestHeaders headers)
    {
        if (AuditLogReason != null)
            headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(AuditLogReason));
    }
}