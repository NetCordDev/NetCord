using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

using NetCord.Gateway.Voice.BinaryModels;
using NetCord.Gateway.Voice.Encryption;
using NetCord.Gateway.Voice.JsonModels;
using NetCord.Gateway.Voice.UdpSockets;
using NetCord.Gateway.WebSockets;
using NetCord.Logging;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway.Voice;

public sealed partial class VoiceClient : WebSocketClient
{
    private class VoiceState(VoiceClient client) : State
    {
        public override void Abort()
        {
            if (client._udpState is { } udpState && udpState.TryIndicateAborting())
                udpState.Connection.Abort();
        }
    }

    internal class UdpState(IUdpConnection connection, IVoiceEncryption encryption, DaveSession daveSession) : IDisposable
    {
        public IUdpConnection Connection => connection;
        public IVoiceEncryption Encryption => encryption;

        public DaveSession DaveSession => daveSession;

        private CancellationTokenProvider? _closedTokenProvider;

        public bool TryIndicateConnecting(out CancellationToken closedCancellationToken)
        {
            CancellationTokenProvider closedTokenProvider = new();
            if (Interlocked.CompareExchange(ref _closedTokenProvider, closedTokenProvider, null) is not null)
            {
                closedTokenProvider.Dispose();
                closedCancellationToken = default;
                return false;
            }

            closedCancellationToken = closedTokenProvider.Token;
            return true;
        }

        public bool TryIndicateAborting()
        {
            CancellationTokenProvider? closedTokenProvider = Interlocked.Exchange(ref _closedTokenProvider, null);
            if (closedTokenProvider is null)
                return false;

            closedTokenProvider.Cancel();
            return true;
        }

        public void Dispose()
        {
            connection.Dispose();
            encryption.Dispose();
            daveSession.Dispose();
            _closedTokenProvider?.Dispose();
        }
    }

    public partial event Func<VoiceReceiveEventArgs, ValueTask>? VoiceReceive;
    public partial event Func<ValueTask>? Ready;
    public partial event Func<SpeakingEventArgs, ValueTask>? Speaking;
    public partial event Func<UserConnectEventArgs, ValueTask>? UserConnect;
    public partial event Func<UserDisconnectEventArgs, ValueTask>? UserDisconnect;

    public ulong UserId { get; }

    public string SessionId { get; }

    public string Endpoint { get; }

    public ulong GuildId { get; }

    public ulong ChannelId { get; }

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

    private readonly IUdpConnectionProvider _udpConnectionProvider;
    private readonly IVoiceEncryptionProvider _encryptionProvider;
    private readonly IVoiceReceiveHandler _receiveHandler;
    private readonly TimeSpan _externalSocketAddressDiscoveryTimeout;
    private readonly GCHandle _loggerHandle;

    internal UdpState? _udpState;

    public VoiceClient(ulong userId, string sessionId, string endpoint, ulong guildId, ulong channelId, string token, VoiceClientConfiguration? configuration = null) : base(configuration ??= new())
    {
        UserId = userId;
        SessionId = sessionId;
        Uri = new($"wss://{Endpoint = endpoint}?v={(int)configuration.Version.GetValueOrDefault(VoiceApiVersion.V8)}", UriKind.Absolute);
        GuildId = guildId;
        ChannelId = channelId;
        Token = token;

        var cacheProvider = configuration.CacheProvider ?? ImmutableVoiceClientCacheProvider.Empty;
        Cache = cacheProvider.Create();
        _udpConnectionProvider = configuration.UdpConnectionProvider ?? UdpConnectionProvider.Instance;
        _encryptionProvider = configuration.EncryptionProvider ?? VoiceEncryptionProvider.Instance;
        _receiveHandler = configuration.ReceiveHandler ?? NullVoiceReceiveHandler.Instance;
        _externalSocketAddressDiscoveryTimeout = configuration.ExternalSocketAddressDiscoveryTimeout.GetValueOrDefault(new(5 * TimeSpan.TicksPerSecond));
        _loggerHandle = GCHandle.Alloc(_logger);
    }

    private protected override ValueTask SendIdentifyAsync(ConnectionState connectionState, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceIdentifyProperties>(VoiceOpcode.Identify, new(GuildId, UserId, SessionId, Token, DaveSession.GetMaxSupportedProtocolVersion())).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceIdentifyProperties);
        _latencyTimer.Start();
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalTextPayloadProperties, cancellationToken);
    }

    private VoiceState CreateState()
    {
        return new(this);
    }

    /// <summary>
    /// Starts the <see cref="VoiceClient"/>.
    /// </summary>
    /// <returns></returns>
    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        var connectionState = await StartAsync(CreateState(), cancellationToken).ConfigureAwait(false);

        Interlocked.Exchange(ref _udpState, null)?.Dispose();

        await SendIdentifyAsync(connectionState, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Resumes the session.
    /// </summary>
    /// <param name="sequenceNumber">The sequence number of the payload to resume from.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns></returns>
    public async ValueTask ResumeAsync(int sequenceNumber, CancellationToken cancellationToken = default)
    {
        var connectionState = await StartAsync(CreateState(), cancellationToken).ConfigureAwait(false);

        if (_udpState is { } udpClient)
        {
            if (udpClient.TryIndicateConnecting(out var closedCancellationToken))
            {
                var connection = udpClient.Connection;

                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                _ = ReadAsync(connection, ArrayPool<byte>.Shared.Rent(ushort.MaxValue), closedCancellationToken);
            }

            await TryResumeAsync(connectionState, SequenceNumber = sequenceNumber, cancellationToken).ConfigureAwait(false);
        }
        else
            await SendIdentifyAsync(connectionState, cancellationToken).ConfigureAwait(false);
    }

    private protected override bool Reconnect(WebSocketCloseStatus? status, string? description)
    {
        return status is not (
            (WebSocketCloseStatus)4004 or
            (WebSocketCloseStatus)4006 or
            (WebSocketCloseStatus)4009 or
            (WebSocketCloseStatus)4011 or
            (WebSocketCloseStatus)4012 or
            (WebSocketCloseStatus)4014 or
            (WebSocketCloseStatus)4016 or
            (WebSocketCloseStatus)4021 or
            (WebSocketCloseStatus)4022
        );
    }

    private protected override ValueTask TryResumeAsync(ConnectionState connectionState, CancellationToken cancellationToken = default)
    {
        return TryResumeAsync(connectionState, SequenceNumber, cancellationToken);
    }

    private ValueTask TryResumeAsync(ConnectionState connectionState, int sequenceNumber, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceResumeProperties>(VoiceOpcode.Resume, new(GuildId, SessionId, Token, sequenceNumber)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceResumeProperties);
        _latencyTimer.Start();
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalTextPayloadProperties, cancellationToken);
    }

    private protected override ValueTask HeartbeatAsync(ConnectionState connectionState, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new VoicePayloadProperties<VoiceHeartbeatProperties>(VoiceOpcode.Heartbeat, new(Environment.TickCount, SequenceNumber)).Serialize(Serialization.Default.VoicePayloadPropertiesVoiceHeartbeatProperties);
        _latencyTimer.Start();
        return SendConnectionPayloadAsync(connectionState, serializedPayload, _internalTextPayloadProperties, cancellationToken);
    }

    private protected override ValueTask ProcessPayloadAsync(State state, ConnectionState connectionState, WebSocketMessageType messageType, ReadOnlySpan<byte> payload)
    {
        if (messageType is WebSocketMessageType.Text)
        {
            var jsonPayload = JsonSerializer.Deserialize(payload, Serialization.Default.JsonVoicePayload)!;
            return HandleJsonPayloadAsync(state, connectionState, jsonPayload);
        }
        else
        {
            BinaryVoicePayload binaryPayload = new(payload);
            return HandleBinaryPayloadAsync(connectionState, binaryPayload);
        }
    }

    private ValueTask HandleBinaryPayloadAsync(ConnectionState connectionState, BinaryVoicePayload payload)
    {
        SequenceNumber = payload.SequencyNumber;

        switch (payload.Opcode)
        {
            case VoiceOpcode.DaveMlsExternalSender:
                {
                    if (_udpState is not { DaveSession: var session })
                        return default;

                    var externalSender = payload.Data;

                    session.OnMlsExternalSender(externalSender);
                }
                return default;
            case VoiceOpcode.DaveMlsProposals:
                {
                    if (_udpState is not { DaveSession: var session })
                        return default;

                    var proposals = payload.Data;

                    return session.OnMlsProposalsAsync(connectionState, proposals);
                }
            case VoiceOpcode.DaveMlsAnnounceCommitTransition:
                {
                    if (_udpState is not { DaveSession: var session })
                        return default;

                    var data = payload.Data;

                    var transitionId = BinaryPrimitives.ReadUInt16BigEndian(data);

                    var commitBytes = data[2..];

                    return session.OnMlsPrepareCommitTransitionAsync(connectionState, transitionId, commitBytes);
                }
            case VoiceOpcode.DaveMlsWelcome:
                {
                    if (_udpState is not { DaveSession: var session })
                        return default;

                    var data = payload.Data;

                    var transitionId = BinaryPrimitives.ReadUInt16BigEndian(data);

                    var welcome = data[2..];

                    return session.OnMlsWelcomeAsync(connectionState, transitionId, welcome);
                }
            default:
                return default;
        }
    }

    private static unsafe void LogMlsFailure(byte* source, byte* reason, void* userData)
    {
        var logger = (IWebSocketLogger)GCHandle.FromIntPtr((nint)userData).Target!;

        if (logger.IsEnabled(LogLevel.Error))
        {
            var sourceStr = Marshal.PtrToStringUTF8((nint)source);
            var reasonStr = Marshal.PtrToStringUTF8((nint)reason);
            logger.Log(LogLevel.Error, (Source: sourceStr, Reason: reasonStr), null, static (s, e) => $"An MLS error occured: {s.Source} {s.Reason}");
        }
    }

    private async ValueTask HandleJsonPayloadAsync(State state, ConnectionState connectionState, JsonVoicePayload payload)
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

                    var (ip, port) = (ready.Ip, ready.Port);
                    var udpConnection = _udpConnectionProvider.CreateConnection(ip, port);
                    var encryption = _encryptionProvider.GetEncryption(ready.Modes);

                    UdpState newUdpState;
                    unsafe
                    {
                        newUdpState = new(udpConnection, encryption, new(this, &LogMlsFailure, (void*)GCHandle.ToIntPtr(_loggerHandle)));
                    }

                    if (Interlocked.CompareExchange(ref _udpState, newUdpState, null) is not null)
                    {
                        newUdpState.Dispose();
                        return;
                    }

                    if (!newUdpState.TryIndicateConnecting(out var closedCancellationToken))
                        return;

                    var ssrc = ready.Ssrc;
                    Cache = Cache.CacheCurrentSsrc(ssrc);

                    var encryptionName = encryption.Name;
                    Log(LogLevel.Debug, encryptionName, null, static (s, e) =>
                    {
                        return $"Using '{s}' encryption.";
                    });

                    await udpConnection.OpenAsync().ConfigureAwait(false);

                    var buffer = ArrayPool<byte>.Shared.Rent(ushort.MaxValue);

                    if (_receiveHandler.RequiresExternalSocketAddress)
                    {
                        Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Getting external socket address.");

                        (ip, port) = await GetExternalSocketAddressAsync(udpConnection, ssrc, buffer).ConfigureAwait(false);
                        if (ip is null)
                        {
                            Log<object?>(LogLevel.Error, null, null, static (s, e) => "Failed to get the external socket address. Aborting the client.");

                            Abort();
                            return;
                        }

                        Log(LogLevel.Debug, (Ip: ip, Port: port), null, static (s, e) => $"External socket address: {s.Ip}:{s.Port}.");
                    }

                    _ = ReadAsync(udpConnection, buffer, closedCancellationToken);

                    Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Selecting a protocol.");

                    VoicePayloadProperties<ProtocolProperties> protocolPayload = new(VoiceOpcode.SelectProtocol, new("udp", new(ip, port, encryptionName)));
                    await SendConnectionPayloadAsync(connectionState, protocolPayload.Serialize(Serialization.Default.VoicePayloadPropertiesProtocolProperties), _internalTextPayloadProperties).ConfigureAwait(false);

                    await updateLatencyTask;
                }
                break;
            case VoiceOpcode.SessionDescription:
                {
                    Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Session description received.");

                    if (_udpState is not { Encryption: var encryption, DaveSession: var session })
                        return;

                    var sessionDescription = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonSessionDescription);

                    encryption.SetKey(sessionDescription.SecretKey);

                    var protocolVersion = sessionDescription.DaveProtocolVersion;

                    await session.OnSessionDescriptionAsync(connectionState, protocolVersion).ConfigureAwait(false);

                    Log<object?>(LogLevel.Information, null, null, static (s, e) => "Ready.");

                    var readyTask = InvokeEventAsync(_ready);

                    state.IndicateReady(connectionState);

                    await readyTask.ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Speaking:
                {
                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonSpeaking);

                    await InvokeEventAsync(_speaking, this, json, static json => new SpeakingEventArgs(json), static (client, json) => client.Cache = client.Cache.CacheUserSsrc(json.UserId, json.Ssrc)).ConfigureAwait(false);

                    if (_udpState is { DaveSession: var session })
                        session.OnSpeaking(json.UserId, json.Ssrc);
                }
                break;
            case VoiceOpcode.HeartbeatACK:
                {
                    var latency = _latencyTimer.Elapsed;

                    Log(LogLevel.Debug, latency, null, static (s, e) =>
                    {
                        return $"Heartbeat acknowledged after {s.TotalMilliseconds:F0} ms.";
                    });

                    await UpdateLatencyAsync(latency).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.Hello:
                {
                    Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Hello received.");

                    StartHeartbeating(connectionState, payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonHello).HeartbeatInterval);
                }
                break;
            case VoiceOpcode.Resumed:
                {
                    var latency = _latencyTimer.Elapsed;

                    Log<object?>(LogLevel.Information, null, null, static (s, e) => "Resumed.");

                    var updateLatencyTask = UpdateLatencyAsync(latency);
                    var resumeTask = InvokeResumeEventAsync();

                    state.IndicateReady(connectionState);

                    await resumeTask.ConfigureAwait(false);
                    await updateLatencyTask.ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.ClientConnect:
                {
                    Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Client connect received.");

                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonClientConnect);
                    await InvokeEventAsync(_userConnect, this, json, static json => new UserConnectEventArgs(json.UserIds), static (client, json) => client.Cache = client.Cache.CacheUsers(json.UserIds)).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.ClientDisconnect:
                {
                    Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Client disconnect received.");

                    var json = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonClientDisconnect);

                    if (_udpState is { DaveSession: var session })
                        session.OnClientDisconnect(json.UserId);

                    await InvokeEventAsync(_userDisconnect, this, json, static json => new UserDisconnectEventArgs(json.UserId), static (client, json) => client.Cache = client.Cache.RemoveUser(json.UserId)).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.DavePrepareTransition:
                {
                    if (_udpState is not { DaveSession: var session })
                        return;

                    var prepareTransition = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonDavePrepareTransition);

                    await session.OnPrepareTransitionAsync(connectionState, prepareTransition.TransitionId, prepareTransition.ProtocolVersion).ConfigureAwait(false);
                }
                break;
            case VoiceOpcode.DaveExecuteTransition:
                {
                    if (_udpState is not { DaveSession: var session })
                        return;

                    var executeTransition = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonDaveExecuteTransition);

                    session.OnExecuteTransition(executeTransition.TransitionId);
                }
                break;
            case VoiceOpcode.DavePrepareEpoch:
                {
                    if (_udpState is not { DaveSession: var session })
                        return;

                    var prepareEpoch = payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonDavePrepareEpoch);

                    await session.OnPrepareEpoch(connectionState, prepareEpoch.Epoch, prepareEpoch.ProtocolVersion).ConfigureAwait(false);
                }
                break;
        }
    }

    private async Task ReadAsync(IUdpConnection udpConnection, byte[] buffer, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                int length = await udpConnection.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                HandleDatagramReceive(new(buffer, 0, length));
            }
        }
        catch
        {
        }

        ArrayPool<byte>.Shared.Return(buffer);
    }

    private async ValueTask<(string? Ip, ushort Port)> GetExternalSocketAddressAsync(IUdpConnection udpConnection, uint ssrc, byte[] buffer)
    {
        var array = ArrayPool<byte>.Shared.Rent(74);

        var discoveryDatagram = CreateDiscoveryDatagram(array, ssrc);

        using CancellationTokenSource cancellationTokenSource = new(_externalSocketAddressDiscoveryTimeout);

        var cancellationToken = cancellationTokenSource.Token;

        int length;
        try
        {
            await udpConnection.SendAsync(discoveryDatagram, cancellationToken).ConfigureAwait(false);

            while (true)
            {
                length = await udpConnection.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

                if (length is 74 && BinaryPrimitives.ReadUInt16BigEndian(buffer) is 2)
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            Log<object?>(LogLevel.Error, null, null, static (s, e) =>
            {
                return "Failed to get the external socket address due to timeout.";
            });
            return default;
        }
        catch (Exception ex)
        {
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while getting the external socket address.{Environment.NewLine}{e}";
            });
            return default;
        }

        ArrayPool<byte>.Shared.Return(array);

        return GetSocketAddress(buffer.AsSpan(0, length));
    }

    private static ReadOnlyMemory<byte> CreateDiscoveryDatagram(byte[] buffer, uint ssrc)
    {
        Memory<byte> bytes = new(buffer, 0, 74);
        var span = bytes.Span;
        BinaryPrimitives.WriteUInt16BigEndian(span, 1);
        BinaryPrimitives.WriteUInt16BigEndian(span[2..], 70);
        BinaryPrimitives.WriteUInt32BigEndian(span[4..], ssrc);
        span[8..].Clear();
        return bytes;
    }

    private static (string Ip, ushort Port) GetSocketAddress(ReadOnlySpan<byte> datagram)
    {
        var ip = Encoding.UTF8.GetString(datagram[8..72].TrimEnd((byte)0));
        var port = BinaryPrimitives.ReadUInt16BigEndian(datagram[72..]);
        return (ip, port);
    }

    private async void HandleDatagramReceive(ReadOnlyMemory<byte> datagram)
    {
        if (_udpState is not { Encryption: var encryption, DaveSession: var session })
            return;

        var handlers = _voiceReceive;
        if (handlers.IsEmpty)
            return;

        try
        {
            RtpPacketStorage packetStorage = new(datagram, encryption.ExtensionEncryption);

            var result = _receiveHandler.HandlePacket(this, GetPacketAndSsrc(packetStorage, out var ssrc));
            if (!result.Handle)
                return;

            if (session.GetDecryptor(ssrc) is not { } decryptor)
                return;

            var framesMissed = result.FramesMissed;

            if (framesMissed is 0)
            {
                await InvokeEventForReceivedFrameAsync().ConfigureAwait(false);
                return;
            }

            var tasks = ArrayPool<ValueTask>.Shared.Rent(framesMissed + 1);

#pragma warning disable CA2012 // Use ValueTasks correctly
            for (ushort i = 0; i < framesMissed; i++)
            {
                tasks[i] = InvokeEventAsync(handlers, ssrc, static ssrc =>
                {
                    return new VoiceReceiveEventArgs(null, 0, 0, ssrc);
                }, nameof(_voiceReceive));
            }

            tasks[framesMissed] = InvokeEventForReceivedFrameAsync();
#pragma warning restore CA2012 // Use ValueTasks correctly

            await HandleTasksThatDoNotThrowAsync(tasks, framesMissed).ConfigureAwait(false);

            ValueTask InvokeEventForReceivedFrameAsync()
            {
                return InvokeEventWithDisposalAsync(handlers, (Encryption: encryption, Decryptor: decryptor, PacketStorage: packetStorage, Ssrc: ssrc), static data =>
                {
                    var (encryption, decryptor, packetStorage, ssrc) = data;

                    var packet = packetStorage.Packet;

                    var plaintextLength = packet.PayloadLength - encryption.Expansion;
                    var array = ArrayPool<byte>.Shared.Rent(plaintextLength);
                    var plaintext = array.AsSpan(0, plaintextLength);
                    encryption.Decrypt(packet, plaintext);

                    var extensionLength = packet.Extension
                        ? 4 * (encryption.ExtensionEncryption
                            ? BinaryPrimitives.ReadUInt16BigEndian(plaintext[2..]) + 1
                            : BinaryPrimitives.ReadUInt16BigEndian(packet.Datagram[(packet.HeaderLength + 2)..]))
                        : 0;

                    int daveArrayLength = decryptor.GetMaxPlaintextByteSize(Dave.MediaType.Audio, plaintextLength - extensionLength);
                    var daveArray = ArrayPool<byte>.Shared.Rent(daveArrayLength);
                    var result = decryptor.Decrypt(Dave.MediaType.Audio, packet.Ssrc, plaintext[extensionLength..], daveArray, out int bytesWritten);

                    ArrayPool<byte>.Shared.Return(array);

                    if (result is Dave.DecryptorResultCode.Success)
                        return new VoiceReceiveEventArgs(daveArray, 0, bytesWritten, ssrc);

                    ArrayPool<byte>.Shared.Return(daveArray);

                    throw new DaveDecryptorException(result);
                }, args =>
                {
                    ArrayPool<byte>.Shared.Return(args._buffer!);
                }, nameof(_voiceReceive));
            }
        }
        catch (Exception ex)
        {
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while handling a datagram.{Environment.NewLine}{e}";
            });
        }

        static RtpPacket GetPacketAndSsrc(RtpPacketStorage packetStorage, out uint ssrc)
        {
            var packet = packetStorage.Packet;
            ssrc = packet.Ssrc;
            return packet;
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private static async ValueTask HandleTasksThatDoNotThrowAsync(ValueTask[] tasks, ushort maxIndex)
    {
        for (ushort i = 0; i <= maxIndex; i++)
            await tasks[i].ConfigureAwait(false);

        ArrayPool<ValueTask>.Shared.Return(tasks);
    }

    public ValueTask EnterSpeakingStateAsync(SpeakingProperties speaking, WebSocketPayloadProperties? properties = null, CancellationToken cancellationToken = default)
    {
        VoicePayloadProperties<SpeakingProperties> payload = new(VoiceOpcode.Speaking, speaking);
        return SendPayloadAsync(payload.Serialize(Serialization.Default.VoicePayloadPropertiesSpeakingProperties), properties, cancellationToken);
    }

    /// <summary>
    /// Creates a stream that you can write to to send voice. Each write must be exactly one Opus frame.
    /// </summary>
    /// <param name="normalizeSpeed">Whether to normalize the voice sending speed.</param>
    /// <returns></returns>
    public Stream CreateOutputStream(bool normalizeSpeed = true)
    {
        Stream stream = new VoiceOutStream(this);
        if (normalizeSpeed)
            stream = new SpeedNormalizingStream(stream);
        return stream;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Cache.Dispose();
            _udpState?.Dispose();
        }

        _loggerHandle.Free();
        base.Dispose(disposing);
    }
}
