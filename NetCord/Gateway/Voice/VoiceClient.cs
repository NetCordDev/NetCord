using System.Buffers.Binary;
using System.Net.Sockets;

using NetCord.Gateway.Voice.Streams;
using NetCord.Gateway.Voice.UdpSockets;
using NetCord.Gateway.WebSockets;
using NetCord.JsonModels;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway.Voice;

public class VoiceClient : WebSocketClient
{
    private readonly IUdpSocket _udpSocket;
    public string Endpoint { get; }
    public Snowflake GuildId { get; }
    public Snowflake UserId { get; }
    public string SessionId { get; }
    public string Token { get; }

    public bool RedirectInputStreams { get; }

    public IReadOnlyDictionary<Snowflake, uint> Ssrcs => _ssrcs;

    private readonly Dictionary<Snowflake, uint> _ssrcs = new(0);

    private readonly Dictionary<uint, InputStream> _inputStreams = new(0);

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;

    public uint Ssrc { get; private set; }

    internal byte[]? _secretKey;

    internal ushort _sequenceNumber;
    internal uint _timestamp;

    public event Func<uint, ReadOnlyMemory<byte>, ValueTask>? VoiceReceive;
    public event Func<ValueTask>? Ready;

    public Task ReadyAsync => _readyCompletionSource.Task;
    private readonly TaskCompletionSource _readyCompletionSource = new();

    public VoiceClient(string endpoint, Snowflake guildId, Snowflake userId, string sessionId, string token, VoiceClientConfig? config = null) : base(config?.WebSocket ?? new WebSocket())
    {
        _cancellationTokenSource = new();
        _cancellationToken = _cancellationTokenSource.Token;
        Endpoint = endpoint;
        GuildId = guildId;
        UserId = userId;
        SessionId = sessionId;
        Token = token;
        if (config != null)
        {
            _udpSocket = config.UdpClient ?? new UdpSocket();
            RedirectInputStreams = config.RedirectInputStreams;
        }
        else
            _udpSocket = new UdpSocket();
    }

    private protected override async Task ProcessMessageAsync(JsonPayload jsonPayload)
    {
        switch ((VoiceOpcode)jsonPayload.Opcode)
        {
            case VoiceOpcode.Ready:
                Latency = _latencyTimer.Elapsed;
                _reconnectTimer.Reset();
                var ready = jsonPayload.Data.GetValueOrDefault().ToObject<JsonModels.JsonReady>();
                Ssrc = ready.Ssrc;

                _udpSocket.Connect(ready.Ip, ready.Port);
                if (RedirectInputStreams)
                {
                    Memory<byte> bytes = new(new byte[70]);
                    bytes.Span[1] = 1;

                    TaskCompletionSource<byte[]> result = new();
                    _udpSocket.DatagramReceive += UdpClient_DatagramReceiveOnce;
                    await _udpSocket.SendAsync(bytes).ConfigureAwait(false);
                    var datagram = await result.Task.ConfigureAwait(false);

                    string ip;
                    ushort port;
                    GetIpAndPort();
                    _udpSocket.DatagramReceive += UdpClient_DatagramReceive;
                    VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ip, port, "xsalsa20_poly1305")));
                    await _webSocket.SendAsync(protocolPayload.Serialize()).ConfigureAwait(false);


                    void UdpClient_DatagramReceiveOnce(UdpReceiveResult datagram)
                    {
                        _udpSocket.DatagramReceive -= UdpClient_DatagramReceiveOnce;
                        result.SetResult(datagram.Buffer);
                    }

                    void GetIpAndPort()
                    {
                        Span<byte> span = new(datagram);
                        ip = System.Text.Encoding.UTF8.GetString(span[4..(64 + 4)].TrimEnd((byte)0));
                        port = BinaryPrimitives.ReadUInt16BigEndian(span[68..]);
                    }
                }
                else
                {
                    VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ready.Ip, ready.Port, "xsalsa20_poly1305")));
                    await _webSocket.SendAsync(protocolPayload.Serialize()).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.SessionDescription:
                var sessionDescription = jsonPayload.Data.GetValueOrDefault().ToObject<JsonModels.JsonSessionDescription>();
                _secretKey = sessionDescription.SecretKey;
                InvokeLog(LogMessage.Info("Ready"));
                try
                {
                    if (Ready != null)
                        await Ready().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    InvokeLog(LogMessage.Error(ex));
                }
                if (!_readyCompletionSource.Task.IsCompleted)
                    _readyCompletionSource.SetResult();
                break;
            case VoiceOpcode.Speaking:
                var speaking = jsonPayload.Data.GetValueOrDefault().ToObject<JsonModels.JsonSpeaking>();
                _ssrcs[speaking.UserId] = speaking.Ssrc;

                ToArrayStream toArrayStream = new();
                OpusDecodeStream opusDecodeStream = new(toArrayStream);
                SodiumDecryptStream sodiumDecryptStream = new(opusDecodeStream, this);
                _inputStreams[speaking.Ssrc] = new(toArrayStream, sodiumDecryptStream);
                break;
            case VoiceOpcode.HeartbeatACK:
                Latency = _latencyTimer.Elapsed;
                break;
            case VoiceOpcode.Hello:
                BeginHeartbeating(jsonPayload.Data.GetValueOrDefault().GetProperty("heartbeat_interval").GetDouble());
                break;
            case VoiceOpcode.Resumed:
                Latency = _latencyTimer.Elapsed;
                _reconnectTimer.Reset();
                InvokeLog(LogMessage.Info("Resumed"));
                break;
            case VoiceOpcode.ClientDisconnect:
                break;
        }
    }

    private async void UdpClient_DatagramReceive(UdpReceiveResult obj)
    {
        if (VoiceReceive != null)
        {
            var ssrc = BinaryPrimitives.ReadUInt32BigEndian(((Span<byte>)obj.Buffer)[8..]);
            if (_inputStreams.TryGetValue(ssrc, out var stream))
            {
                _ = stream.WriteStream.WriteAsync(obj.Buffer); //it's sync
                await VoiceReceive(ssrc, stream.Stream.Data!).ConfigureAwait(false);
            }
        }
    }

    private protected override Task HeartbeatAsync()
    {
        var serializedPayload = new VoicePayloadProperties<int>(VoiceOpcode.Heartbeat, Environment.TickCount).Serialize();
        _latencyTimer.Start();
        return _webSocket.SendAsync(serializedPayload, _cancellationToken);
    }

    public async Task StartAsync()
    {
        await _webSocket.ConnectAsync(new Uri($"wss://{Endpoint}?v=4")).ConfigureAwait(false);
        await SendIdentifyAsync().ConfigureAwait(false);
    }

    public async Task CloseAsync()
    {
        await _webSocket.CloseAsync().ConfigureAwait(false);
        _udpSocket.Dispose();
    }

    public async Task EnterSpeakingStateAsync(SpeakingFlags flags, int delay = 0)
    {
        VoicePayloadProperties<SpeakingProperties> payload = new(VoiceOpcode.Speaking, new(flags, delay, Ssrc));
        await _webSocket.SendAsync(payload.Serialize()).ConfigureAwait(false);
    }

    private Task SendIdentifyAsync()
    {
        var payload = new VoicePayloadProperties<VoiceIdentifyProperties>(VoiceOpcode.Identify, new(GuildId, UserId, SessionId, Token));
        _latencyTimer.Start();
        return _webSocket.SendAsync(payload.Serialize());
    }

    public Stream CreatePCMStream(OpusApplication application)
    {
        VoiceStream voiceStream = new(_udpSocket);
        SodiumEncryptStream sodiumEncryptStream = new(voiceStream, this);
        OpusEncodeStream opusEncodeStream = new(sodiumEncryptStream, application);
        SpeedNormalizingStream speedNormalizingStream = new(opusEncodeStream);
        SegmentingStream segmentingStream = new(speedNormalizingStream);
        return segmentingStream;
    }

    public Stream CreateDirectPCMStream(OpusApplication application)
    {
        VoiceStream voiceStream = new(_udpSocket);
        SodiumEncryptStream sodiumEncryptStream = new(voiceStream, this);
        OpusEncodeStream opusEncodeStream = new(sodiumEncryptStream, application);
        SegmentingStream segmentingStream = new(opusEncodeStream);
        return segmentingStream;
    }

    public override void Dispose()
    {
        _udpSocket.Dispose();
        foreach (var stream in _inputStreams.Values)
            stream.WriteStream.Dispose();
        base.Dispose();
    }

    private protected override ValueTask ReconnectAsync(WebSocketCloseStatus? status, string? description)
    {
        if (status is not (WebSocketCloseStatus)4004 or (WebSocketCloseStatus)4006 or (WebSocketCloseStatus)4009 or (WebSocketCloseStatus)4014)
            return base.ReconnectAsync(status, description);
        else
            return default;
    }

    private protected override async Task ResumeAsync()
    {
        await _webSocket.ConnectAsync(new Uri($"wss://{Endpoint}?v=4")).ConfigureAwait(false);
        var serializedPayload = new VoicePayloadProperties<ResumeProperties>(VoiceOpcode.Resume, new(GuildId, SessionId, Token)).Serialize();
        _latencyTimer.Start();
        await _webSocket.SendAsync(serializedPayload).ConfigureAwait(false);
    }
}