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
    private readonly Uri _url;

    public string Endpoint { get; }
    public ulong GuildId { get; }
    public ulong UserId { get; }
    public string SessionId { get; }
    public string Token { get; }

    public bool RedirectInputStreams { get; }

    public IReadOnlyDictionary<uint, ulong> Users => _users;

    private readonly Dictionary<uint, ulong> _users = new(0);

    private readonly Dictionary<uint, Stream> _inputStreams = new(0);

    public uint Ssrc { get; private set; }

    internal byte[]? _secretKey;

    public event Func<VoiceReceiveEventArgs, ValueTask>? VoiceReceive;
    public event Func<ValueTask>? Ready;

    public Task ReadyAsync => _readyCompletionSource.Task;
    private readonly TaskCompletionSource _readyCompletionSource = new();

    public VoiceClient(ulong userId, string sessionId, string endpoint, ulong guildId, string token, VoiceClientConfiguration? configuration = null) : base((configuration ??= new()).WebSocket ?? new WebSocket())
    {
        UserId = userId;
        SessionId = sessionId;
        _url = new($"wss://{Endpoint = endpoint}?v={(int)configuration.Version}", UriKind.Absolute);
        GuildId = guildId;
        Token = token;

        _udpSocket = configuration.UdpSocket ?? new UdpSocket();
        RedirectInputStreams = configuration.RedirectInputStreams;
    }

    private ValueTask SendIdentifyAsync()
    {
        var serializedPayload = new VoicePayloadProperties<VoiceIdentifyProperties>(VoiceOpcode.Identify, new(GuildId, UserId, SessionId, Token)).Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfVoiceIdentifyPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesVoiceIdentifyProperties);
        _latencyTimer.Start();
        return _webSocket.SendAsync(serializedPayload);
    }

    public async Task StartAsync()
    {
        await _webSocket.ConnectAsync(_url).ConfigureAwait(false);
        await SendIdentifyAsync().ConfigureAwait(false);
    }

    public async Task CloseAsync()
    {
        await CloseAsync(WebSocketCloseStatus.NormalClosure).ConfigureAwait(false);
        _udpSocket.Dispose();
    }

    private protected override bool Reconnect(WebSocketCloseStatus? status, string? description)
        => status is not ((WebSocketCloseStatus)4004 or (WebSocketCloseStatus)4006 or (WebSocketCloseStatus)4009 or (WebSocketCloseStatus)4014);

    private protected override async Task ResumeAsync()
    {
        await _webSocket.ConnectAsync(_url).ConfigureAwait(false);
        var serializedPayload = new VoicePayloadProperties<VoiceResumeProperties>(VoiceOpcode.Resume, new(GuildId, SessionId, Token)).Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfVoiceResumePropertiesSerializerContext.WithOptions.VoicePayloadPropertiesVoiceResumeProperties);
        _latencyTimer.Start();
        await _webSocket.SendAsync(serializedPayload).ConfigureAwait(false);
    }

    private protected override ValueTask HeartbeatAsync()
    {
        var serializedPayload = new VoicePayloadProperties<int>(VoiceOpcode.Heartbeat, Environment.TickCount).Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfInt32SerializerContext.WithOptions.VoicePayloadPropertiesInt32);
        _latencyTimer.Start();
        return _webSocket.SendAsync(serializedPayload);
    }

    private protected override async Task ProcessMessageAsync(JsonPayload payload)
    {
        switch ((VoiceOpcode)payload.Opcode)
        {
            case VoiceOpcode.Ready:
                {
                    var latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    await UpdateLatencyAsync(latency).ConfigureAwait(false);
                    var ready = payload.Data.GetValueOrDefault().ToObject(JsonModels.JsonReady.JsonReadySerializerContext.WithOptions.JsonReady);
                    Ssrc = ready.Ssrc;

                    _udpSocket.Connect(ready.Ip, ready.Port);
                    if (RedirectInputStreams)
                    {
                        TaskCompletionSource<byte[]> result = new();
                        _udpSocket.DatagramReceive += UdpSocket_DatagramReceiveOnce;
                        await _udpSocket.SendAsync(CreateDatagram()).ConfigureAwait(false);

                        var datagram = await result.Task.ConfigureAwait(false);
                        GetIpAndPort(out var ip, out var port);

                        _udpSocket.DatagramReceive += UdpSocket_DatagramReceive;

                        VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ip, port, "xsalsa20_poly1305")));
                        await _webSocket.SendAsync(protocolPayload.Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfProtocolPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesProtocolProperties)).ConfigureAwait(false);

                        ReadOnlyMemory<byte> CreateDatagram()
                        {
                            Memory<byte> bytes = new(new byte[74]);
                            var span = bytes.Span;
                            span[1] = 1;
                            span[3] = 70;
                            BinaryPrimitives.WriteUInt32BigEndian(span[4..], Ssrc);
                            return bytes;
                        }

                        void UdpSocket_DatagramReceiveOnce(UdpReceiveResult datagram)
                        {
                            _udpSocket.DatagramReceive -= UdpSocket_DatagramReceiveOnce;
                            result.SetResult(datagram.Buffer);
                        }

                        void GetIpAndPort(out string ip, out ushort port)
                        {
                            Span<byte> span = new(datagram);
                            ip = System.Text.Encoding.UTF8.GetString(span[8..72].TrimEnd((byte)0));
                            port = BinaryPrimitives.ReadUInt16BigEndian(span[72..]);
                        }
                    }
                    else
                    {
                        VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ready.Ip, ready.Port, "xsalsa20_poly1305")));
                        await _webSocket.SendAsync(protocolPayload.Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfProtocolPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesProtocolProperties)).ConfigureAwait(false);
                    }
                }
                break;
            case VoiceOpcode.SessionDescription:
                {
                    var sessionDescription = payload.Data.GetValueOrDefault().ToObject(JsonModels.JsonSessionDescription.JsonSessionDescriptionSerializerContext.WithOptions.JsonSessionDescription);
                    _secretKey = sessionDescription.SecretKey;
                    InvokeLog(LogMessage.Info("Ready"));
                    var updateLatencyTask = InvokeEventAsync(Ready);

                    if (!_readyCompletionSource.Task.IsCompleted)
                        _readyCompletionSource.SetResult();

                    await updateLatencyTask.ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Speaking:
                {
                    var speaking = payload.Data.GetValueOrDefault().ToObject(JsonModels.JsonSpeaking.JsonSpeakingSerializerContext.WithOptions.JsonSpeaking);
                    var ssrc = speaking.Ssrc;
                    var userId = speaking.UserId;
                    _users[ssrc] = userId;

                    VoiceInStream voiceInStream = new(this, ssrc, userId);
                    OpusDecodeStream opusDecodeStream = new(voiceInStream);
                    SodiumDecryptStream sodiumDecryptStream = new(opusDecodeStream, this);
                    _inputStreams[ssrc] = sodiumDecryptStream;
                }
                break;
            case VoiceOpcode.HeartbeatACK:
                {
                    await UpdateLatencyAsync(_latencyTimer.Elapsed).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Hello:
                {
                    BeginHeartbeating(payload.Data.GetValueOrDefault().GetProperty("heartbeat_interval").GetDouble());
                }
                break;
            case VoiceOpcode.Resumed:
                {
                    var latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    InvokeLog(LogMessage.Info("Resumed"));
                    var updateLatencyTask = UpdateLatencyAsync(latency).ConfigureAwait(false);
                    await InvokeResumedEventAsync().ConfigureAwait(false);
                    await updateLatencyTask;
                }
                break;
            case VoiceOpcode.ClientDisconnect:
                break;
        }
    }

    internal ValueTask InvokeVoiceReceiveAsync(VoiceReceiveEventArgs data) => InvokeEventAsync(VoiceReceive, data);

    private async void UdpSocket_DatagramReceive(UdpReceiveResult obj)
    {
        var @event = VoiceReceive;
        if (@event != null)
        {
            try
            {
                ReadOnlyMemory<byte> buffer = obj.Buffer;
                var ssrc = BinaryPrimitives.ReadUInt32BigEndian(buffer.Span[8..]);
                if (_inputStreams.TryGetValue(ssrc, out var stream))
                    await stream.WriteAsync(buffer).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        }
    }

    public ValueTask EnterSpeakingStateAsync(SpeakingFlags flags, int delay = 0)
    {
        VoicePayloadProperties<SpeakingProperties> payload = new(VoiceOpcode.Speaking, new(flags, delay, Ssrc));
        return _webSocket.SendAsync(payload.Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfSpeakingPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesSpeakingProperties));
    }

    public Stream CreatePCMStream(OpusApplication application)
    {
        VoiceOutStream voiceOutStream = new(_udpSocket);
        SodiumEncryptStream sodiumEncryptStream = new(voiceOutStream, this);
        OpusEncodeStream opusEncodeStream = new(sodiumEncryptStream, application);
        SpeedNormalizingStream speedNormalizingStream = new(opusEncodeStream);
        SegmentingStream segmentingStream = new(speedNormalizingStream);
        return segmentingStream;
    }

    public Stream CreateDirectPCMStream(OpusApplication application)
    {
        VoiceOutStream voiceOutStream = new(_udpSocket);
        SodiumEncryptStream sodiumEncryptStream = new(voiceOutStream, this);
        OpusEncodeStream opusEncodeStream = new(sodiumEncryptStream, application);
        SegmentingStream segmentingStream = new(opusEncodeStream);
        return segmentingStream;
    }

    public override void Dispose()
    {
        _udpSocket.Dispose();
        foreach (var stream in _inputStreams.Values)
            stream.Dispose();
        base.Dispose();
    }
}
