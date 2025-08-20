using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

[GenerateMethodsForProperties]
public partial record WebSocketPayloadProperties
{
    /// <summary>
    /// The type of message to send. Defaults to <see cref="WebSocketMessageType.Text"/>.
    /// </summary>
    public WebSocketMessageType? MessageType { get; set; }

    /// <summary>
    /// The flags to apply to the message. Defaults to <see cref="WebSocketMessageFlags.EndOfMessage"/>.
    /// </summary>
    public WebSocketMessageFlags? MessageFlags { get; set; }

    /// <summary>
    /// The retry handling to apply to the message. Defaults to <see cref="WebSocketRetryHandling.Retry"/>.
    /// </summary>
    public WebSocketRetryHandling? RetryHandling { get; set; }
}
