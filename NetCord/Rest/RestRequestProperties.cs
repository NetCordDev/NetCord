namespace NetCord.Rest;

public partial class RestRequestProperties
{
    public RestRateLimitHandling RateLimitHandling { get; set; } = RestRateLimitHandling.Retry;
    public string? AuditLogReason { get; set; }
    public string? ErrorLocalization { get; set; }

    internal record struct RestRequestHeaderInfo(string Name, IEnumerable<string> Values);

    internal IEnumerable<RestRequestHeaderInfo> GetHeaders()
    {
        var auditLogReason = AuditLogReason;
        if (auditLogReason is not null)
            yield return new("X-Audit-Log-Reason", [Uri.EscapeDataString(auditLogReason)]);

        var errorLocalization = ErrorLocalization;
        if (errorLocalization is not null)
            yield return new("Accept-Language", [Uri.EscapeDataString(errorLocalization)]);
    }

    internal RestRequestProperties WithoutHeaders()
    {
        return new()
        {
            RateLimitHandling = RateLimitHandling,
        };
    }
}
