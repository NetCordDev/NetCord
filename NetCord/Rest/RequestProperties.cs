using System.Net.Http.Headers;

namespace NetCord.Rest;

public partial class RequestProperties
{
    public string? AuditLogReason { get; set; }
    public RateLimitHandling RateLimitHandling { get; set; }

    internal void AddHeaders(HttpRequestHeaders headers)
    {
        if (AuditLogReason != null)
            headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(AuditLogReason));
    }
}
