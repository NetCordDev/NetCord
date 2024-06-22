using System.Buffers.Binary;
using System.Net.Sockets;

using NetCord.Gateway.JsonModels;
using NetCord.Gateway.Voice.Encryption;
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
    public IVoiceClientCache Cache { get; private set; }

    private protected override Uri Uri { get; }

    private readonly Dictionary<uint, Stream> _inputStreams = [];
    private readonly IUdpSocket _udpSocket;
    private readonly IVoiceEncryption _encryption;

    public VoiceClient(ulong userId, string sessionId, string endpoint, ulong guildId, string token, VoiceClientConfiguration? configuration = null) : base(configuration ??= new())
    {
        UserId = userId;
        SessionId = sessionId;
        Uri = new($"wss://{Endpoint = endpoint}?v={(int)configuration.Version}", UriKind.Absolute);
        GuildId = guildId;
        Token = token;

        _udpSocket = configuration.UdpSocket ?? new UdpSocket();
        Cache = configuration.Cache ?? new VoiceClientCache();
        _encryption = configuration.Encryption ?? new Aes256GcmRtpSizeEncryption();
        RedirectInputStreams = configuration.RedirectInputStreams;
    }

    private ValueTask SendIdentifyAsync(CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceIdentifyProperties>(VoiceOpcode.Identify, new(GuildId, UserId, SessionId, Token)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceIdentifyProperties);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload, cancellationToken);
    }

    /// <summary>
    /// Starts the <see cref="VoiceClient"/>.
    /// </summary>
    /// <returns></returns>
    public async new Task StartAsync(CancellationToken cancellationToken = default)
    {
        await base.StartAsync(cancellationToken).ConfigureAwait(false);
        await SendIdentifyAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Resumes a session.
    /// </summary>
    /// <returns></returns>
    public async Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        await ConnectAsync(cancellationToken).ConfigureAwait(false);
        await TryResumeAsync(cancellationToken).ConfigureAwait(false);
    }

    private protected override bool Reconnect(WebSocketCloseStatus? status, string? description)
        => status is not ((WebSocketCloseStatus)4004 or (WebSocketCloseStatus)4006 or (WebSocketCloseStatus)4009 or (WebSocketCloseStatus)4014);

    private protected override ValueTask TryResumeAsync(CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceResumeProperties>(VoiceOpcode.Resume, new(GuildId, SessionId, Token)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceResumeProperties);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload, cancellationToken);
    }

    private protected override ValueTask HeartbeatAsync(CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<int>(VoiceOpcode.Heartbeat, Environment.TickCount).Serialize(Serialization.Default.VoicePayloadPropertiesInt32);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload, cancellationToken);
    }

    private protected override async Task ProcessPayloadAsync(JsonPayload payload)
    {
        switch ((VoiceOpcode)payload.Opcode)
        {
            case VoiceOpcode.Ready:
                {
                    var latency = _latencyTimer.Elapsed;
                    await UpdateLatencyAsync(latency).ConfigureAwait(false);
                    var ready = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonReady);
                    var ssrc = ready.Ssrc;
                    Cache = Cache.CacheCurrentSsrc(ssrc);

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
                        await SendPayloadAsync(protocolPayload.Serialize(Serialization.Default.VoicePayloadPropertiesProtocolProperties)).ConfigureAwait(false);

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
                        await SendPayloadAsync(protocolPayload.Serialize(Serialization.Default.VoicePayloadPropertiesProtocolProperties)).ConfigureAwait(false);
                    }
                }
                break;
            case VoiceOpcode.SessionDescription:
                {
                    var sessionDescription = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonSessionDescription);
                    _encryption.SetKey(sessionDescription.SecretKey);
                    InvokeLog(LogMessage.Info("Ready"));
                    var readyTask = InvokeEventAsync(Ready);

                    _readyCompletionSource.TrySetResult();

                    await readyTask.ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Speaking:
                {
                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonSpeaking);
                    var ssrc = json.Ssrc;
                    var userId = json.UserId;
                    Cache = Cache.CacheUser(ssrc, userId);

                    VoiceInStream voiceInStream = new(this, ssrc, userId);
                    DecryptStream decryptStream = new(voiceInStream, _encryption);
                    if (_inputStreams.Remove(ssrc, out var stream))
                        stream.Dispose();
                    _inputStreams[ssrc] = decryptStream;
                }
                break;
            case VoiceOpcode.HeartbeatACK:
                {
                    await UpdateLatencyAsync(_latencyTimer.Elapsed).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Hello:
                {
                    StartHeartbeating(payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonHello).HeartbeatInterval);
                }
                break;
            case VoiceOpcode.Resumed:
                {
                    var latency = _latencyTimer.Elapsed;
                    InvokeLog(LogMessage.Info("Resumed"));
                    var updateLatencyTask = UpdateLatencyAsync(latency).ConfigureAwait(false);
                    await InvokeResumeEventAsync().ConfigureAwait(false);
                    await updateLatencyTask;
                }
                break;
            case VoiceOpcode.ClientDisconnect:
                {
                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonClientDisconnect);
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

    public ValueTask EnterSpeakingStateAsync(SpeakingFlags flags, int delay = 0, CancellationToken cancellationToken = default)
    {
        VoicePayloadProperties<SpeakingProperties> payload = new(VoiceOpcode.Speaking, new(flags, delay, Cache.Ssrc));
        return SendPayloadAsync(payload.Serialize(Serialization.Default.VoicePayloadPropertiesSpeakingProperties), cancellationToken);
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _udpSocket.Dispose();
            Cache.Dispose();
            foreach (var stream in _inputStreams.Values)
                stream.Dispose();
            _encryption.Dispose();
        }
        base.Dispose(disposing);
    }
}
