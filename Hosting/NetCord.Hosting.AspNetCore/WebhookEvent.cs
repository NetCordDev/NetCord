namespace NetCord.Hosting.AspNetCore;

public partial class WebhookEvent
{
    internal WebhookEvent(WebhookEventId eventId)
    {
        EventId = eventId;
    }

    internal WebhookEventId EventId { get; }
}

public partial class WebhookEvent<T>
{
    internal WebhookEvent(WebhookEventId eventId)
    {
        EventId = eventId;
    }

    internal WebhookEventId EventId { get; }
}
