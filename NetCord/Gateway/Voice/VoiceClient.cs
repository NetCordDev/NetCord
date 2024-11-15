using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using NetCord.Gateway.Voice.Encryption;
using NetCord.Gateway.Voice.JsonModels;
using NetCord.Gateway.Voice.UdpSockets;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway.Voice;

public class VoiceClient : WebSocketClient
{
    public event Func<VoiceReceiveEventArgs, ValueTask>? VoiceReceive;
    public event Func<ValueTask>? Ready;
    public event Func<UserConnectEventArgs, ValueTask>? UserConnect;
    public event Func<UserDisconnectEventArgs, ValueTask>? UserDisconnect;

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

    /// <summary>
    /// The sequence number of the <see cref="VoiceClient"/>.
    /// </summary>
    public int SequenceNumber { get; private set; } = -1;

    private protected override Uri Uri { get; }

    private readonly Dictionary<uint, Stream> _inputStreams = [];
    private readonly IUdpSocket _udpSocket;
    private readonly IVoiceEncryption _encryption;

    public VoiceClient(ulong userId, string sessionId, string endpoint, ulong guildId, string token, VoiceClientConfiguration? configuration = null) : base(configuration ??= new())
    {
        UserId = userId;
        SessionId = sessionId;
        Uri = new($"wss://{Endpoint = endpoint}?v={(int)configuration.Version.GetValueOrDefault(VoiceApiVersion.V8)}", UriKind.Absolute);
        GuildId = guildId;
        Token = token;

        _udpSocket = configuration.UdpSocket ?? new UdpSocket();
        Cache = configuration.Cache ?? new VoiceClientCache();
        _encryption = configuration.Encryption ?? new Aes256GcmRtpSizeEncryption();
        RedirectInputStreams = configuration.RedirectInputStreams.GetValueOrDefault(false);
    }

    private ValueTask SendIdentifyAsync(ConnectionState connectionState, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceIdentifyProperties>(VoiceOpcode.Identify, new(GuildId, UserId, SessionId, Token)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceIdentifyProperties);
        _latencyTimer.Start();
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalPayloadProperties, cancellationToken);
    }

    /// <summary>
    /// Starts the <see cref="VoiceClient"/>.
    /// </summary>
    /// <returns></returns>
    public async new Task StartAsync(CancellationToken cancellationToken = default)
    {
        var connectionState = await base.StartAsync(cancellationToken).ConfigureAwait(false);
        await SendIdentifyAsync(connectionState, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Resumes the session.
    /// </summary>
    /// <param name="sequenceNumber">The sequence number of the payload to resume from.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns></returns>
    public async Task ResumeAsync(int sequenceNumber, CancellationToken cancellationToken = default)
    {
        var connectionState = await base.StartAsync(cancellationToken).ConfigureAwait(false);
        await TryResumeAsync(connectionState, SequenceNumber = sequenceNumber, cancellationToken).ConfigureAwait(false);
    }

    private protected override bool Reconnect(WebSocketCloseStatus? status, string? description)
        => status is not ((WebSocketCloseStatus)4004 or (WebSocketCloseStatus)4006 or (WebSocketCloseStatus)4009 or (WebSocketCloseStatus)4014);

    private protected override ValueTask TryResumeAsync(ConnectionState connectionState, CancellationToken cancellationToken = default)
    {
        return TryResumeAsync(connectionState, SequenceNumber, cancellationToken);
    }

    private ValueTask TryResumeAsync(ConnectionState connectionState, int sequenceNumber, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceResumeProperties>(VoiceOpcode.Resume, new(GuildId, SessionId, Token, sequenceNumber)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceResumeProperties);
        _latencyTimer.Start();
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalPayloadProperties, cancellationToken);
    }

    private protected override ValueTask HeartbeatAsync(ConnectionState connectionState, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceHeartbeatProperties>(VoiceOpcode.Heartbeat, new(Environment.TickCount, SequenceNumber)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceHeartbeatProperties);
        _latencyTimer.Start();
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalPayloadProperties, cancellationToken);
    }

    private protected override Task ProcessPayloadAsync(State state, ConnectionState connectionState, ReadOnlySpan<byte> payload)
    {
        var jsonPayload = JsonSerializer.Deserialize(payload, Serialization.Default.JsonVoicePayload)!;
        return HandlePayloadAsync(state, connectionState, jsonPayload);
    }

    private async Task HandlePayloadAsync(State state, ConnectionState connectionState, JsonVoicePayload payload)
    {
        if (payload.SequenceNumber is int sequenceNumber)
            SequenceNumber = sequenceNumber;

        switch (payload.Opcode)
        {
            case VoiceOpcode.Ready:
                {
                    var latency = _latencyTimer.Elapsed;
                    var updateLatencyTask = UpdateLatencyAsync(latency).ConfigureAwait(false);
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
                        await SendConnectionPayloadAsync(connectionState, protocolPayload.Serialize(Serialization.Default.VoicePayloadPropertiesProtocolProperties), _internalPayloadProperties).ConfigureAwait(false);

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
                            ip = Encoding.UTF8.GetString(span[8..72].TrimEnd((byte)0));
                            port = BinaryPrimitives.ReadUInt16BigEndian(span[72..]);
                        }
                    }
                    else
                    {
                        VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ready.Ip, ready.Port, _encryption.Name)));
                        await SendConnectionPayloadAsync(connectionState, protocolPayload.Serialize(Serialization.Default.VoicePayloadPropertiesProtocolProperties), _internalPayloadProperties).ConfigureAwait(false);
                    }

                    await updateLatencyTask;
                }
                break;
            case VoiceOpcode.SessionDescription:
                {
                    var sessionDescription = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonSessionDescription);
                    _encryption.SetKey(sessionDescription.SecretKey);
                    InvokeLog(LogMessage.Info("Ready"));
                    var readyTask = InvokeEventAsync(Ready);

                    state.IndicateReady(connectionState);

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

                    var inputStreams = _inputStreams;
                    if (inputStreams.Remove(ssrc, out var stream))
                        stream.Dispose();
                    inputStreams[ssrc] = decryptStream;
                }
                break;
            case VoiceOpcode.HeartbeatACK:
                {
                    await UpdateLatencyAsync(_latencyTimer.Elapsed).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Hello:
                {
                    StartHeartbeating(connectionState, payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonHello).HeartbeatInterval);
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
            case VoiceOpcode.ClientConnect:
                {
                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonClientConnect);
                    await InvokeEventAsync(UserConnect, new UserConnectEventArgs(json.UserIds)).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.ClientDisconnect:
                {
                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonClientDisconnect);
                    await InvokeEventAsync(UserDisconnect, new(json.UserId), args =>
                    {
                        var userId = args.UserId;
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

    public ValueTask EnterSpeakingStateAsync(SpeakingFlags flags, int delay = 0, WebSocketPayloadProperties? properties = null, CancellationToken cancellationToken = default)
    {
        VoicePayloadProperties<SpeakingProperties> payload = new(VoiceOpcode.Speaking, new(flags, delay, Cache.Ssrc));
        return SendPayloadAsync(payload.Serialize(Serialization.Default.VoicePayloadPropertiesSpeakingProperties), properties, cancellationToken);
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
