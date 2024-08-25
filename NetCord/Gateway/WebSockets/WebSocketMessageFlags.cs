namespace NetCord.Gateway.WebSockets;

[Flags]
public enum WebSocketMessageFlags : byte
{
    None = 0,
    EndOfMessage = 1 << 0,
    DisableCompression = 1 << 1,
    BypassReady = 1 << 7,
}
