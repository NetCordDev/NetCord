namespace NetCord.Rest;

/// <summary>
/// Represents the type of a <see cref="Webhook"/>.
/// </summary>
public enum WebhookType
{
    /// <summary>
    /// Incoming webhooks can post messages to channels with a generated token.
    /// </summary>
    Incoming = 1,

    /// <summary>
    /// Channel Follower Webhooks are internal webhooks used with Channel Following to post new messages into channels.
    /// </summary>
    ChannelFollower = 2,

    /// <summary>
    /// Application webhooks are webhooks used via the <see cref="Interaction"/> system.
    /// </summary>
    Application = 3,
}
