namespace NetCord.Rest;

/// <summary>
/// Contains configuration info for a <see cref="WebhookClient"/>.
/// </summary>
public class WebhookClientConfiguration
{
    /// <summary>
    /// The client to use as a base for the webhook client.
    /// </summary>
    public RestClient? Client { get; set; }
}
