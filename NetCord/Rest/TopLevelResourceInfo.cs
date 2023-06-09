namespace NetCord.Rest;

public readonly record struct TopLevelResourceInfo
{
    public TopLevelResourceInfo(ulong resourceId, string? resourceToken = null)
    {
        ResourceId = resourceId;
        ResourceToken = resourceToken;
    }

    public ulong ResourceId { get; }

    public string? ResourceToken { get; }
}
