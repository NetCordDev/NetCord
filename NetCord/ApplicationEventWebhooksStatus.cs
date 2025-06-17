namespace NetCord;

public enum ApplicationEventWebhooksStatus
{
    /// <summary>
    /// Webhook events are disabled by developer.
    /// </summary>
    Disabled = 1,

    /// <summary>
    /// Webhook events are enabled by developer.
    /// </summary>
    Enabled = 2,

    /// <summary>
    /// Webhook events are disabled by Discord, usually due to inactivity.
    /// </summary>
    DisabledByDiscord = 3,
}
