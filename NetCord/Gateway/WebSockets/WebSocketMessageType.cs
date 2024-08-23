namespace NetCord.Gateway.WebSockets;

public enum WebSocketMessageType : byte
{
    Text = 0,
    Binary = 1,
    Close = 2,
}
