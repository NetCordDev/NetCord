namespace NetCord.Hosting.AspNetCore;

public static partial class WebhookEvent;

public partial class WebhookEvent<T>
{
    internal WebhookEvent(string? rawName)
    {
        RawName = rawName;
    }

    internal string? RawName { get; }
}
