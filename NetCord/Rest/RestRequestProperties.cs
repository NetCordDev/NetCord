using System.Net.Http.Headers;

namespace NetCord.Rest;

public partial class RestRequestProperties
{
    public RestRateLimitHandling RateLimitHandling { get; set; } = RestRateLimitHandling.Retry;
    public string? AuditLogReason { get; set; }
    public IEnumerable<StringWithQualityHeaderValue>? ErrorLanguage { get; set; }

    internal record struct RestRequestHeaderInfo(string Name, IEnumerable<string> Values);

    internal IEnumerable<RestRequestHeaderInfo> GetHeaders()
    {
        var auditLogReason = AuditLogReason;
        if (auditLogReason is not null)
            yield return new("X-Audit-Log-Reason", [Uri.EscapeDataString(auditLogReason)]);

        var errorLanguage = ErrorLanguage;
        if (errorLanguage is not null)
            yield return new("Accept-Language", errorLanguage.Select(l => l.ToString()));
    }

    internal RestRequestProperties WithoutHeaders()
    {
        return new()
        {
            RateLimitHandling = RateLimitHandling,
        };
    }
}
