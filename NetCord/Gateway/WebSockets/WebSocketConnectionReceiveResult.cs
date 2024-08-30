namespace NetCord.Gateway.WebSockets;

#pragma warning disable IDE0032 // Use auto property

// Adopted from System.Net.WebSockets.ValueWebSocketReceiveResult

public readonly struct WebSocketConnectionReceiveResult
{
    private readonly uint _countAndEndOfMessage;
    private readonly WebSocketMessageType _messageType;

    public WebSocketConnectionReceiveResult(int count, WebSocketMessageType messageType, bool endOfMessage)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count, nameof(count));
        if ((uint)messageType > (uint)WebSocketMessageType.Close)
            ThrowMessageTypeOutOfRange();

        _countAndEndOfMessage = (uint)count | (uint)(endOfMessage ? 1 << 31 : 0);
        _messageType = messageType;

        static void ThrowMessageTypeOutOfRange() => throw new ArgumentOutOfRangeException(nameof(messageType));
    }

    public int Count => (int)(_countAndEndOfMessage & 0x7FFFFFFF);
    public bool EndOfMessage => (_countAndEndOfMessage & 0x80000000) == 0x80000000;
    public WebSocketMessageType MessageType => _messageType;
}
