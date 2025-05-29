using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using NetCord.Gateway.Voice.Encryption;
using NetCord.Gateway.Voice.JsonModels;
using NetCord.Gateway.Voice.UdpSockets;
using NetCord.Logging;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway.Voice;

public partial class VoiceClient : WebSocketClient
{
    public partial event Func<VoiceReceiveEventArgs, ValueTask>? VoiceReceive;
    public partial event Func<ValueTask>? Ready;
    public partial event Func<SpeakingEventArgs, ValueTask>? Speaking;
    public partial event Func<UserConnectEventArgs, ValueTask>? UserConnect;
    public partial event Func<UserDisconnectEventArgs, ValueTask>? UserDisconnect;

    public ulong UserId { get; }

    public string SessionId { get; }

    public string Endpoint { get; }

    public ulong GuildId { get; }

    public string Token { get; }

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
    private readonly IVoiceEncryptionProvider _encryptionProvider;
    private readonly IVoiceReceiveHandler _receiveHandler;

    private IVoiceEncryption? _encryption;

    public VoiceClient(ulong userId, string sessionId, string endpoint, ulong guildId, string token, VoiceClientConfiguration? configuration = null) : base(configuration ??= new())
    {
        UserId = userId;
        SessionId = sessionId;
        Uri = new($"wss://{Endpoint = endpoint}?v={(int)configuration.Version.GetValueOrDefault(VoiceApiVersion.V8)}", UriKind.Absolute);
        GuildId = guildId;
        Token = token;

        _udpSocket = configuration.UdpSocket ?? new UdpSocket();
        Cache = configuration.Cache ?? new VoiceClientCache();
        _encryptionProvider = configuration.EncryptionProvider ?? VoiceEncryptionProvider.Instance;
        _receiveHandler = configuration.ReceiveHandler ?? NullVoiceReceiveHandler.Instance;
    }

    private ValueTask SendIdentifyAsync(ConnectionState connectionState, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceIdentifyProperties>(VoiceOpcode.Identify, new(GuildId, UserId, SessionId, Token)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceIdentifyProperties);
        _latencyTimer.Start();
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalPayloadProperties, _logger, cancellationToken);
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
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalPayloadProperties, _logger, cancellationToken);
    }

    private protected override ValueTask HeartbeatAsync(ConnectionState connectionState, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceHeartbeatProperties>(VoiceOpcode.Heartbeat, new(Environment.TickCount, SequenceNumber)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceHeartbeatProperties);
        _latencyTimer.Start();
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalPayloadProperties, _logger, cancellationToken);
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
                    var encryption = _encryption = _encryptionProvider.GetEncryption(ready.Modes);
                    var encryptionName = encryption.Name;

                    _logger.Log(LogLevel.Debug, encryptionName, null, static (s, e) =>
                    {
                        return $"Using '{s}' encryption.";
                    });

                    Cache = Cache.CacheCurrentSsrc(ssrc);

                    var udpSocket = _udpSocket;

                    udpSocket.Connect(ready.Ip, ready.Port);

                    _logger.Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Getting external socket address.");
                    
                    var (hostname, port) = await GetExternalSocketAddressAsync(udpSocket, ssrc).ConfigureAwait(false);

                    _logger.Log(LogLevel.Debug, (Hostname: hostname, Port: port), null, static (s, e) => $"External socket address: {s.Hostname}:{s.Port}.");

                    udpSocket.DatagramReceive += HandleDatagramReceive;

                    _logger.Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Selecting a protocol.");

                    VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(hostname, port, encryptionName)));
                    await SendConnectionPayloadAsync(connectionState, protocolPayload.Serialize(Serialization.Default.VoicePayloadPropertiesProtocolProperties), _internalPayloadProperties, _logger).ConfigureAwait(false);

                    await updateLatencyTask;
                }
                break;
            case VoiceOpcode.SessionDescription:
                {
                    _logger.Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Session description received.");

                    var sessionDescription = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonSessionDescription);
                    _encryption!.SetKey(sessionDescription.SecretKey);

                    _logger.Log<object?>(LogLevel.Information, null, null, static (s, e) => "Ready.");

                    var readyTask = InvokeEventAsync(_ready);

                    state.IndicateReady(connectionState);

                    await readyTask.ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Speaking:
                {
                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonSpeaking);

                    await InvokeEventAsync(_speaking, () => new SpeakingEventArgs(json)).ConfigureAwait(false);

                    //var ssrc = json.Ssrc;
                    //var userId = json.UserId;
                    //Cache = Cache.CacheUser(ssrc, userId);

                    //VoiceInStream voiceInStream = new(this, ssrc, userId);
                    //DecryptStream decryptStream = new(voiceInStream, _encryption!);

                    //var inputStreams = _inputStreams;
                    //if (inputStreams.Remove(ssrc, out var stream))
                    //    stream.Dispose();
                    //inputStreams[ssrc] = decryptStream;
                }
                break;
            case VoiceOpcode.HeartbeatACK:
                {
                    var latency = _latencyTimer.Elapsed;

                    _logger.Log(LogLevel.Debug, latency, null, static (s, e) =>
                    {
                        return $"Heartbeat acknowledged after {s.TotalMilliseconds:F0} ms.";
                    });

                    await UpdateLatencyAsync(latency).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Hello:
                {
                    _logger.Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Hello received.");

                    StartHeartbeating(connectionState, payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonHello).HeartbeatInterval);
                }
                break;
            case VoiceOpcode.Resumed:
                {
                    var latency = _latencyTimer.Elapsed;

                    _logger.Log<object?>(LogLevel.Information, null, null, static (s, e) => "Resumed.");

                    var updateLatencyTask = UpdateLatencyAsync(latency).ConfigureAwait(false);
                    await InvokeResumeEventAsync().ConfigureAwait(false);
                    await updateLatencyTask;
                }
                break;
            case VoiceOpcode.ClientConnect:
                {
                    _logger.Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Client connect received.");

                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonClientConnect);
                    await InvokeEventAsync(_userConnect, new UserConnectEventArgs(json.UserIds)).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.ClientDisconnect:
                {
                    _logger.Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Client disconnect received.");

                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonClientDisconnect);
                    await InvokeEventAsync(_userDisconnect, new(json.UserId), args =>
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

    public async ValueTask<(string Hostname, ushort Port)> GetExternalSocketAddressAsync(IUdpSocket udpSocket, uint ssrc)
    {
        TaskCompletionSource<byte[]> receiveCompletionSource = new();

        var receiveSocketAddress = ReceiveSocketAddress;

        udpSocket.DatagramReceive += receiveSocketAddress;

        var array = ArrayPool<byte>.Shared.Rent(74);

        var discoveryDatagram = CreateDiscoveryDatagram(array, ssrc);

        await udpSocket.SendAsync(discoveryDatagram).ConfigureAwait(false);

        ArrayPool<byte>.Shared.Return(array);

        var datagram = await receiveCompletionSource.Task.ConfigureAwait(false);

        udpSocket.DatagramReceive -= receiveSocketAddress;

        return GetSocketAddress(datagram);

        void ReceiveSocketAddress(UdpReceiveResult result)
        {
            receiveCompletionSource.TrySetResult(result.Buffer);
        }
    }

    private static ReadOnlyMemory<byte> CreateDiscoveryDatagram(byte[] buffer, uint ssrc)
    {
        Memory<byte> bytes = new(buffer, 0, 74);
        var span = bytes.Span;
        span[1] = 1;
        span[3] = 70;
        BinaryPrimitives.WriteUInt32BigEndian(span[4..], ssrc);
        return bytes;
    }

    private static (string Hostname, ushort Port) GetSocketAddress(byte[] datagram)
    {
        Span<byte> span = new(datagram);
        var hostname = Encoding.UTF8.GetString(span[8..72].TrimEnd((byte)0));
        var port = BinaryPrimitives.ReadUInt16BigEndian(span[72..]);
        return (hostname, port);
    }

    private void HandleDatagramReceive(UdpReceiveResult receiveResult)
    {
        try
        {
            var encryption = _encryption!;
            RtpPacketStorage packetStorage = new(receiveResult.Buffer, encryption.ExtensionEncryption);

            var result = _receiveHandler.HandlePacket(packetStorage.Packet);
            if (!result.Handle)
                return;

            var handlers = _voiceReceive;

            var framesMissed = result.FramesMissed;

            if (framesMissed is 0)
            {
                _ = InvokeEventForReceivedFrameAsync().AsTask();
                return;
            }

            var tasks = ArrayPool<ValueTask>.Shared.Rent(framesMissed + 1);

#pragma warning disable CA2012 // Use ValueTasks correctly
            for (ushort i = 0; i < framesMissed; i++)
            {
                tasks[i] = InvokeEventAsync(handlers, () =>
                {
                    var packet = packetStorage.Packet;
                    return new VoiceReceiveEventArgs(null, 0, 0, packet.Ssrc);
                }, nameof(_voiceReceive));
            }

            tasks[framesMissed] = InvokeEventForReceivedFrameAsync();
#pragma warning restore CA2012 // Use ValueTasks correctly

            _ = HandleTasksThatDoNotThrowAsync(tasks, framesMissed);

            ValueTask InvokeEventForReceivedFrameAsync()
            {
                return InvokeEventWithDisposalAsync(handlers, () =>
                {
                    var packet = packetStorage.Packet;
                    var plaintextLength = packet.PayloadLength - encryption.Expansion;
                    var array = ArrayPool<byte>.Shared.Rent(plaintextLength);
                    encryption.Decrypt(packet, array.AsSpan(0, plaintextLength));

                    var extensionLength = packet.Extension
                        ? 4 * (encryption.ExtensionEncryption
                            ? BinaryPrimitives.ReadUInt16BigEndian(packet.Datagram[2..]) + 1
                            : BinaryPrimitives.ReadUInt16BigEndian(packet.Datagram[(packet.HeaderLength + 2)..]))
                        : 0;

                    return new VoiceReceiveEventArgs(array, extensionLength, plaintextLength - extensionLength, packet.Ssrc);
                }, args =>
                {
                    ArrayPool<byte>.Shared.Return(args._buffer!);
                }, nameof(_voiceReceive));
            }
        }
        catch (Exception ex)
        {
            _logger.Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while handling a datagram.{Environment.NewLine}{e}";
            });
        }

        //ReadOnlyMemory<byte> buffer = obj.Buffer;
        //var ssrc = BinaryPrimitives.ReadUInt32BigEndian(buffer.Span[8..]);
        //var encryption = _encryption!;
        //RtpPacketStorage packetStorage = new(receiveResult.Buffer, encryption.ExtensionEncryption);

        //if (packetStorage.Packet.PayloadType is not 0x78)
        //    return;

        //await InvokeEventAsync(_voiceReceive, () =>
        //{
        //    return new VoiceReceiveEventArgs(packetStorage, encryption);
        //}).ConfigureAwait(false);

        //var handlers = _voiceReceive;
        //if (!handlers.IsEmpty)
        //{
        //    try
        //    {
        //        ReadOnlyMemory<byte> buffer = obj.Buffer;
        //var ssrc = BinaryPrimitives.ReadUInt32BigEndian(buffer.Span[8..]);
        //        if (_inputStreams.TryGetValue(ssrc, out var stream))
        //            await stream.WriteAsync(buffer).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
        //        {
        //            return $"An error occurred while handling a datagram.{Environment.NewLine}{e}";
        //        });
        //    }
        //}
    }

    private static async Task HandleTasksThatDoNotThrowAsync(ValueTask[] tasks, ushort maxIndex)
    {
        for (ushort i = 0; i <= maxIndex; i++)
            await tasks[i].ConfigureAwait(false);

        ArrayPool<ValueTask>.Shared.Return(tasks);
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
    /// <exception cref="InvalidOperationException">Thrown when invoked before the <see cref="Ready"/> event.</exception>
    public Stream CreateOutputStream(bool normalizeSpeed = true)
    {
        if (_encryption is not { } encryption)
            throw new InvalidOperationException($"The output stream cannot be created before the {nameof(Ready)} event.");

        Stream stream = new VoiceOutStream(_udpSocket);
        if (normalizeSpeed)
            stream = new SpeedNormalizingStream(stream);
        stream = new EncryptStream(stream, encryption, this);
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
            _encryption?.Dispose();
        }
        base.Dispose(disposing);
    }
}
