using System.Net.WebSockets;
using System.Text;

namespace NetCord.WebSockets;

public class WebSocket
{
    private readonly Uri _uri;
    private ClientWebSocket _webSocket;
    private readonly Memory<byte> _receiveBuffer = new(new byte[128]);
    private bool _closed;
    private Task _readAsync;

    public event Action Connecting;
    public event Action Connected;
    public event DisconnectedEventHandler Disconnected;
    public event Action Closed;

    public event MessageReceivedEventHandler MessageReceived;

    public delegate void DisconnectedEventHandler(WebSocketCloseStatus? closeStatus, string? closeStatusDescription);
    public delegate void MessageReceivedEventHandler(MemoryStream data);

    /// <summary>
    /// Gets or sets the default encoding
    /// </summary>
    public Encoding Encoding { get; init; } = Encoding.UTF8;

    /// <summary>
    /// Creates a new instance of <see cref="WebSocket"/>
    /// </summary>
    /// <param name="uri"></param>
    public WebSocket(Uri uri)
    {
        _uri = uri;
    }

    /// <summary>
    /// Connect to a WebSocket server
    /// </summary>
    /// <returns></returns>
    public async Task ConnectAsync()
    {
        try
        {
            Connecting?.Invoke();
        }
        finally
        {
            await (_webSocket = new()).ConnectAsync(_uri, default).ConfigureAwait(false);
            try
            {
                Connected?.Invoke();
            }
            finally
            {
                _closed = false;
                _readAsync = ReadAsync();
            }
        }
    }

    /// <summary>
    /// Close the <see cref="WebSocket"/>
    /// </summary>
    public async Task CloseAsync()
    {
        _closed = true;
        await _webSocket.CloseAsync(WebSocketCloseStatus.Empty, null, default).ConfigureAwait(false);
        await _readAsync.ConfigureAwait(false);
    }

    /// <summary>
    /// Send a message
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public async Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token = default)
    {
        await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send a message
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public async Task SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageFlags flags, CancellationToken token = default)
    {
        await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, flags, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send a message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task SendAsync(string message, CancellationToken token = default)
    {
        ReadOnlyMemory<byte> messageBytes = new(Encoding.GetBytes(message));
        return SendAsync(messageBytes, token);
    }

    /// <summary>
    /// Send a message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public Task SendAsync(string message, WebSocketMessageFlags flags, CancellationToken token = default)
    {
        ReadOnlyMemory<byte> messageBytes = new(Encoding.GetBytes(message));
        return SendAsync(messageBytes, flags, token);
    }

    private async Task ReadAsync()
    {
        try
        {
            while (true)
            {
                using MemoryStream stream = new();
                while (true)
                {
                    var r = await _webSocket.ReceiveAsync(_receiveBuffer, default).ConfigureAwait(false);
                    if (r.EndOfMessage)
                    {
                        if (r.MessageType != WebSocketMessageType.Close)
                        {
                            await stream.WriteAsync(_receiveBuffer[..r.Count]).ConfigureAwait(false);
                            try
                            {
                                MessageReceived?.Invoke(stream);
                            }
                            catch
                            {
                            }
                        }
                        break;
                    }
                    else
                        await stream.WriteAsync(_receiveBuffer).ConfigureAwait(false);
                }
            }
        }
        catch
        {
            try
            {
                if (_closed)
                    Closed?.Invoke();
                else
                    Disconnected?.Invoke(_webSocket.CloseStatus, _webSocket.CloseStatusDescription);
            }
            catch
            {
            }
        }
    }
}
