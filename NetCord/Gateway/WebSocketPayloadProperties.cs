using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public partial record WebSocketPayloadProperties
{
    public WebSocketMessageType MessageType { get; set; }
    public WebSocketMessageFlags MessageFlags { get; set; } = WebSocketMessageFlags.EndOfMessage;
    public WebSocketRetryHandling RetryHandling { get; set; } = WebSocketRetryHandling.Retry;
}
