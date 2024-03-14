using System.Net.Http.Headers;

namespace NetCord.Rest;

public partial class RestRequestProperties
{
    public string? AuditLogReason { get; set; }
    public RateLimitHandling RateLimitHandling { get; set; } = RateLimitHandling.Retry;
    public IEnumerable<StringWithQualityHeaderValue>? ErrorLanguage { get; set; }

    internal void AddHeaders(HttpRequestHeaders headers)
    {
        var auditLogReason = AuditLogReason;
        if (auditLogReason is not null)
            headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(auditLogReason));

        var errorLanguage = ErrorLanguage;
        if (errorLanguage is not null)
        {
            var acceptLanguage = headers.AcceptLanguage;
            foreach (var language in errorLanguage)
                acceptLanguage.Add(language);
        }
    }
}
