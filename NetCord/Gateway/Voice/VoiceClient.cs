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
    public ulong GuildId { get; }
    public ulong UserId { get; }
    public string SessionId { get; }
    public string Token { get; }

    public bool RedirectInputStreams { get; }

    public IReadOnlyDictionary<uint, ulong> Users => _users;

    private readonly Dictionary<uint, ulong> _users = new(0);

    private readonly Dictionary<uint, InputStream> _inputStreams = new(0);

    public uint Ssrc { get; private set; }

    internal byte[]? _secretKey;

    internal ushort _sequenceNumber;
    internal uint _timestamp;

    public event Func<uint, ReadOnlyMemory<byte>, ValueTask>? VoiceReceive;
    public event Func<ValueTask>? Ready;

    public Task ReadyAsync => _readyCompletionSource.Task;
    private readonly TaskCompletionSource _readyCompletionSource = new();

    public VoiceClient(ulong userId, string sessionId, string endpoint, ulong guildId, string token, VoiceClientConfig? config = null) : base(config?.WebSocket ?? new WebSocket())
    {
        UserId = userId;
        SessionId = sessionId;
        Endpoint = endpoint;
        GuildId = guildId;
        Token = token;
        if (config != null)
        {
            _udpSocket = config.UdpSocket ?? new UdpSocket();
            RedirectInputStreams = config.RedirectInputStreams;
        }
        else
            _udpSocket = new UdpSocket();
    }

    private ValueTask SendIdentifyAsync()
    {
        var serializedPayload = new VoicePayloadProperties<VoiceIdentifyProperties>(VoiceOpcode.Identify, new(GuildId, UserId, SessionId, Token)).Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfVoiceIdentifyPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesVoiceIdentifyProperties);
        _latencyTimer.Start();
        return _webSocket.SendAsync(serializedPayload);
    }

    public async Task StartAsync()
    {
        await _webSocket.ConnectAsync(new Uri($"wss://{Endpoint}?v=4")).ConfigureAwait(false);
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
        await _webSocket.ConnectAsync(new Uri($"wss://{Endpoint}?v=4")).ConfigureAwait(false);
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

    private protected override async Task ProcessMessageAsync(JsonPayload jsonPayload)
    {
        switch ((VoiceOpcode)jsonPayload.Opcode)
        {
            case VoiceOpcode.Ready:
                {
                    var latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    await UpdateLatencyAsync(latency).ConfigureAwait(false);
                    var ready = jsonPayload.Data.GetValueOrDefault().ToObject(JsonModels.JsonReady.JsonReadySerializerContext.WithOptions.JsonReady);
                    Ssrc = ready.Ssrc;

                    _udpSocket.Connect(ready.Ip, ready.Port);
                    if (RedirectInputStreams)
                    {
                        Memory<byte> bytes = new(new byte[70]);
                        bytes.Span[1] = 1;

                        TaskCompletionSource<byte[]> result = new();
                        _udpSocket.DatagramReceive += UdpSocket_DatagramReceiveOnce;
                        await _udpSocket.SendAsync(bytes).ConfigureAwait(false);
                        var datagram = await result.Task.ConfigureAwait(false);

                        string ip;
                        ushort port;
                        GetIpAndPort();
                        _udpSocket.DatagramReceive += UdpSocket_DatagramReceive;
                        VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ip, port, "xsalsa20_poly1305")));
                        await _webSocket.SendAsync(protocolPayload.Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfProtocolPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesProtocolProperties)).ConfigureAwait(false);

                        void UdpSocket_DatagramReceiveOnce(UdpReceiveResult datagram)
                        {
                            _udpSocket.DatagramReceive -= UdpSocket_DatagramReceiveOnce;
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
                        await _webSocket.SendAsync(protocolPayload.Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfProtocolPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesProtocolProperties)).ConfigureAwait(false);
                    }
                }

                break;
            case VoiceOpcode.SessionDescription:
                {
                    var sessionDescription = jsonPayload.Data.GetValueOrDefault().ToObject(JsonModels.JsonSessionDescription.JsonSessionDescriptionSerializerContext.WithOptions.JsonSessionDescription);
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
                    var speaking = jsonPayload.Data.GetValueOrDefault().ToObject(JsonModels.JsonSpeaking.JsonSpeakingSerializerContext.WithOptions.JsonSpeaking);
                    _users[speaking.Ssrc] = speaking.UserId;

                    ToMemoryStream toMemoryStream = new();
                    OpusDecodeStream opusDecodeStream = new(toMemoryStream);
                    SodiumDecryptStream sodiumDecryptStream = new(opusDecodeStream, this);
                    _inputStreams[speaking.Ssrc] = new(toMemoryStream, sodiumDecryptStream);
                }
                break;
            case VoiceOpcode.HeartbeatACK:
                {
                    await UpdateLatencyAsync(_latencyTimer.Elapsed).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Hello:
                {
                    BeginHeartbeating(jsonPayload.Data.GetValueOrDefault().GetProperty("heartbeat_interval").GetDouble());
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

    private async void UdpSocket_DatagramReceive(UdpReceiveResult obj)
    {
        if (VoiceReceive != null)
        {
            try
            {
                var ssrc = BinaryPrimitives.ReadUInt32BigEndian(obj.Buffer.AsSpan(8));
                if (_inputStreams.TryGetValue(ssrc, out var stream))
                {
                    _ = stream.WriteStream.WriteAsync(obj.Buffer); //it's sync
                    await VoiceReceive(ssrc, stream.Stream.Data).ConfigureAwait(false);
                }
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
}
