using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public partial record WebSocketPayloadProperties
{
    public WebSocketMessageType? MessageType { get; set; }
    public WebSocketMessageFlags? MessageFlags { get; set; }
    public WebSocketRetryHandling? RetryHandling { get; set; }
}
