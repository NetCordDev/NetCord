namespace NetCord.Rest;

public sealed partial class RestRequestProperties
{
    /// <summary>
    /// The rate limit handling. Defaults to <see cref="RestRateLimitHandling.Retry"/>.
    /// </summary>
    public RestRateLimitHandling? RateLimitHandling { get; set; }

    /// <summary>
    /// The reason for the guild's audit log.
    /// </summary>
    public string? AuditLogReason { get; set; }

    /// <summary>
    /// The error localization. This is used to localize error messages returned by Discord.
    /// </summary>
    public string? ErrorLocalization { get; set; }

    internal readonly record struct RestRequestHeaderInfo(string Name, IEnumerable<string> Values);

    internal IEnumerable<RestRequestHeaderInfo> GetHeaders()
    {
        var auditLogReason = AuditLogReason;
        if (auditLogReason is not null)
            yield return new("X-Audit-Log-Reason", [Uri.EscapeDataString(auditLogReason)]);

        var errorLocalization = ErrorLocalization;
        if (errorLocalization is not null)
            yield return new("Accept-Language", [Uri.EscapeDataString(errorLocalization)]);
    }
}
