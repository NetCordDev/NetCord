using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

using NetCord.Gateway.JsonModels;
using NetCord.Gateway.Voice.Encryption;
using NetCord.Gateway.Voice.JsonModels;
using NetCord.Gateway.Voice.UdpSockets;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway.Voice;

public class VoiceClient : WebSocketClient
{
    public event Func<VoiceReceiveEventArgs, ValueTask>? VoiceReceive;
    public event Func<ValueTask>? Ready;
    public event Func<ulong, ValueTask>? UserDisconnect;

    public ulong UserId { get; }

    public string SessionId { get; }

    public string Endpoint { get; }

    public ulong GuildId { get; }

    public string Token { get; }

    public bool RedirectInputStreams { get; }

    /// <summary>
    /// The cache of the <see cref="VoiceClient"/>.
    /// </summary>
    /// <remarks>It is <see langword="null"/> before starting of the <see cref="VoiceClient"/>.</remarks>
    [AllowNull]
    public IVoiceClientCache Cache { get; private set; }

    private readonly Dictionary<uint, Stream> _inputStreams = new();
    private readonly Uri _url;
    private readonly IUdpSocket _udpSocket;
    private readonly IVoiceEncryption _encryption;

    public VoiceClient(ulong userId, string sessionId, string endpoint, ulong guildId, string token, VoiceClientConfiguration? configuration = null) : base((configuration ??= new()).WebSocket, configuration.ReconnectTimer, configuration.LatencyTimer)
    {
        UserId = userId;
        SessionId = sessionId;
        _url = new($"wss://{Endpoint = endpoint}?v={(int)configuration.Version}", UriKind.Absolute);
        GuildId = guildId;
        Token = token;

        _udpSocket = configuration.UdpSocket ?? new UdpSocket();
        RedirectInputStreams = configuration.RedirectInputStreams;
        _encryption = configuration.Encryption ?? new XSalsa20Poly1305Encryption();
    }

    private ValueTask SendIdentifyAsync()
    {
        var serializedPayload = new VoicePayloadProperties<VoiceIdentifyProperties>(VoiceOpcode.Identify, new(GuildId, UserId, SessionId, Token)).Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfVoiceIdentifyPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesVoiceIdentifyProperties);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload);
    }

    /// <summary>
    /// Starts the <see cref="VoiceClient"/>.
    /// </summary>
    /// <param name="cache">The cache to use.</param>
    /// <returns></returns>
    public async Task StartAsync(IVoiceClientCache? cache = null)
    {
        if (cache is null)
            Cache ??= new VoiceClientCache();
        else
            Cache = cache;

        await ConnectAsync(_url).ConfigureAwait(false);
        await SendIdentifyAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Resumes a session.
    /// </summary>
    /// <param name="cache">The cache to use.</param>
    /// <returns></returns>
    public Task ResumeAsync(IVoiceClientCache? cache = null)
    {
        if (cache is null)
            Cache ??= new VoiceClientCache();
        else
            Cache = cache;

        return TryResumeAsync();
    }

    private protected override bool Reconnect(WebSocketCloseStatus? status, string? description)
        => status is not ((WebSocketCloseStatus)4004 or (WebSocketCloseStatus)4006 or (WebSocketCloseStatus)4009 or (WebSocketCloseStatus)4014);

    private protected override async Task TryResumeAsync()
    {
        await ConnectAsync(_url).ConfigureAwait(false);
        var serializedPayload = new VoicePayloadProperties<VoiceResumeProperties>(VoiceOpcode.Resume, new(GuildId, SessionId, Token)).Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfVoiceResumePropertiesSerializerContext.WithOptions.VoicePayloadPropertiesVoiceResumeProperties);
        _latencyTimer.Start();
        await SendPayloadAsync(serializedPayload).ConfigureAwait(false);
    }

    private protected override ValueTask HeartbeatAsync()
    {
        var serializedPayload = new VoicePayloadProperties<int>(VoiceOpcode.Heartbeat, Environment.TickCount).Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfInt32SerializerContext.WithOptions.VoicePayloadPropertiesInt32);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload);
    }

    private protected override async Task ProcessPayloadAsync(JsonPayload payload)
    {
        switch ((VoiceOpcode)payload.Opcode)
        {
            case VoiceOpcode.Ready:
                {
                    var latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    await UpdateLatencyAsync(latency).ConfigureAwait(false);
                    var ready = payload.Data.GetValueOrDefault().ToObject(JsonReady.JsonReadySerializerContext.WithOptions.JsonReady);
                    var ssrc = ready.Ssrc;
                    Cache = Cache.CacheSelfSsrc(ssrc);

                    _udpSocket.Connect(ready.Ip, ready.Port);
                    if (RedirectInputStreams)
                    {
                        TaskCompletionSource<byte[]> result = new();
                        var handleDatagramReceiveOnce = HandleDatagramReceiveOnce;
                        _udpSocket.DatagramReceive += handleDatagramReceiveOnce;

                        await _udpSocket.SendAsync(CreateDatagram()).ConfigureAwait(false);

                        var datagram = await result.Task.ConfigureAwait(false);
                        _udpSocket.DatagramReceive -= handleDatagramReceiveOnce;

                        GetIpAndPort(out var ip, out var port);

                        _udpSocket.DatagramReceive += HandleDatagramReceive;

                        VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ip, port, _encryption.Name)));
                        await SendPayloadAsync(protocolPayload.Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfProtocolPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesProtocolProperties)).ConfigureAwait(false);

                        ReadOnlyMemory<byte> CreateDatagram()
                        {
                            Memory<byte> bytes = new(new byte[74]);
                            var span = bytes.Span;
                            span[1] = 1;
                            span[3] = 70;
                            BinaryPrimitives.WriteUInt32BigEndian(span[4..], ssrc);
                            return bytes;
                        }

                        void HandleDatagramReceiveOnce(UdpReceiveResult datagram)
                        {
                            result.TrySetResult(datagram.Buffer);
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
                        VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ready.Ip, ready.Port, _encryption.Name)));
                        await SendPayloadAsync(protocolPayload.Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfProtocolPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesProtocolProperties)).ConfigureAwait(false);
                    }
                }
                break;
            case VoiceOpcode.SessionDescription:
                {
                    var sessionDescription = payload.Data.GetValueOrDefault().ToObject(JsonSessionDescription.JsonSessionDescriptionSerializerContext.WithOptions.JsonSessionDescription);
                    _encryption.SetKey(sessionDescription.SecretKey);
                    InvokeLog(LogMessage.Info("Ready"));
                    var readyTask = InvokeEventAsync(Ready);

                    _readyCompletionSource.TrySetResult();

                    await readyTask.ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Speaking:
                {
                    var json = payload.Data.GetValueOrDefault().ToObject(JsonSpeaking.JsonSpeakingSerializerContext.WithOptions.JsonSpeaking);
                    var ssrc = json.Ssrc;
                    var userId = json.UserId;
                    Cache = Cache.CacheUser(ssrc, userId);

                    VoiceInStream voiceInStream = new(this, ssrc, userId);
                    DecryptStream sodiumDecryptStream = new(voiceInStream, _encryption, this);
                    if (_inputStreams.Remove(ssrc, out var stream))
                        stream.Dispose();
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
                    BeginHeartbeating(payload.Data.GetValueOrDefault().ToObject(JsonHello.JsonHelloSerializerContext.WithOptions.JsonHello).HeartbeatInterval);
                }
                break;
            case VoiceOpcode.Resumed:
                {
                    var latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    InvokeLog(LogMessage.Info("Resumed"));
                    var updateLatencyTask = UpdateLatencyAsync(latency).ConfigureAwait(false);
                    await InvokeResumeEventAsync().ConfigureAwait(false);
                    await updateLatencyTask;
                }
                break;
            case VoiceOpcode.ClientDisconnect:
                {
                    var json = payload.Data.GetValueOrDefault().ToObject(JsonClientDisconnect.JsonClientDisconnectSerializerContext.WithOptions.JsonClientDisconnect);
                    await InvokeEventAsync(UserDisconnect, json.UserId, userId =>
                    {
                        var cache = Cache;
                        if (cache.Ssrcs.TryGetValue(userId, out var ssrc))
                        {
                            Cache = cache.RemoveUser(ssrc, userId);
                            if (_inputStreams.Remove(ssrc, out var stream))
                                stream.Dispose();
                        }
                    }).ConfigureAwait(false);
                }
                break;
        }
    }

    internal ValueTask InvokeVoiceReceiveAsync(VoiceReceiveEventArgs data) => InvokeEventAsync(VoiceReceive, data);

    private async void HandleDatagramReceive(UdpReceiveResult obj)
    {
        var @event = VoiceReceive;
        if (@event is not null)
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
        VoicePayloadProperties<SpeakingProperties> payload = new(VoiceOpcode.Speaking, new(flags, delay, Cache.Ssrc));
        return SendPayloadAsync(payload.Serialize(VoicePayloadProperties.VoicePayloadPropertiesOfSpeakingPropertiesSerializerContext.WithOptions.VoicePayloadPropertiesSpeakingProperties));
    }

    /// <summary>
    /// Creates a stream that you can write to to send voice. Each write must be exactly one Opus frame.
    /// </summary>
    /// <param name="normalizeSpeed">Whether to normalize the voice sending speed.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Used before <see cref="Ready"/> event.</exception>
    public Stream CreateOutputStream(bool normalizeSpeed = true)
    {
        Stream stream = new VoiceOutStream(_udpSocket);
        if (normalizeSpeed)
            stream = new SpeedNormalizingStream(stream);
        stream = new EncryptStream(stream, _encryption, this);
        return stream;
    }

    public override void Dispose()
    {
        base.Dispose();
        _udpSocket.Dispose();
        Cache.Dispose();
        foreach (var stream in _inputStreams.Values)
            stream.Dispose();
    }
}
