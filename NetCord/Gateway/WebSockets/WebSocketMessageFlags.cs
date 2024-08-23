namespace NetCord.Gateway.WebSockets;

public enum WebSocketMessageFlags : byte
{
    None = 0,
    EndOfMessage = 1,
    DisableCompression = 2,
}
