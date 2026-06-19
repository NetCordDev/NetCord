using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static NetCord.Gateway.Voice.Dave;

namespace NetCord.Gateway.Voice;

#pragma warning disable IDE0290 // Use primary constructor

public partial class VoiceClient
{
    internal readonly ref struct DaveEncryptor(EncryptorHandle encryptor)
    {
        public readonly EncryptorResultCode Encrypt(MediaType mediaType, uint ssrc, ReadOnlySpan<byte> frame, Span<byte> encryptedFrame, out int bytesWritten)
        {
            var result = EncryptorEncrypt(encryptor, mediaType, ssrc, frame, (uint)frame.Length, encryptedFrame, (nuint)encryptedFrame.Length, out var rawBytesWritten);

            bytesWritten = (int)rawBytesWritten;
            return result;
        }

        public readonly int GetMaxCiphertextSize(MediaType mediaType, int plaintextByteSize)
            => (int)EncryptorGetMaxCiphertextByteSize(encryptor, mediaType, (nuint)plaintextByteSize);
    }

    internal struct DaveDecryptor(DecryptorHandle decryptor)
    {
        public readonly DecryptorResultCode Decrypt(MediaType mediaType, uint ssrc, ReadOnlySpan<byte> encryptedFrame, Span<byte> frame, out int bytesWritten)
        {
            var result = DecryptorDecrypt(decryptor, mediaType, encryptedFrame, (nuint)encryptedFrame.Length, frame, (nuint)frame.Length, out var rawBytesWritten);

            bytesWritten = (int)rawBytesWritten;
            return result;
        }

        public readonly int GetMaxPlaintextByteSize(MediaType mediaType, int ciphertextByteSize)
            => (int)DecryptorGetMaxPlaintextByteSize(decryptor, mediaType, (nuint)ciphertextByteSize);
    }

    internal class DaveSession : IDisposable
    {
        private const int MaxSnowflakeCStringSize = 21;

        private const int MlsNewGroupExpectedEpoch = 1;

        private ushort _latestPreparedTransitionVersion;

        private readonly Dictionary<ushort, ushort> _transitions;

        private readonly EncryptorHandle _encryptor;
        private readonly ConcurrentDictionary<uint, DecryptorHandle> _decryptors;

        private readonly VoiceClient _client;
        private readonly SessionHandle _session;

        public unsafe DaveSession(VoiceClient client, delegate* unmanaged<byte*, byte*, void*, void> mlsFailureCallback, void* userData)
        {
            _transitions = [];

            _encryptor = EncryptorCreate();
            _decryptors = [];

            _client = client;
            _session = SessionCreate(null, null, mlsFailureCallback, userData);
        }

        static unsafe DaveSession()
        {
            SetLogSinkCallback(&LogSink);
        }

        [UnmanagedCallersOnly]
        private static unsafe void LogSink(LoggingSeverity severity, byte* file, int line, byte* message)
        {
#if DEBUG
            var fileString = Marshal.PtrToStringUTF8((nint)file);
            var messageString = Marshal.PtrToStringUTF8((nint)message);
            Debug.WriteLine($"Dave at {fileString}:{line}: {messageString}");
#endif
        }

        public bool IsEnabled => GetProtocolVersion() is not DisabledVersion;

        public DaveEncryptor Encryptor => new(_encryptor);

        public static ushort GetMaxSupportedProtocolVersion()
        {
            return MaxSupportedProtocolVersion();
        }

        public ushort GetProtocolVersion()
        {
            return SessionGetProtocolVersion(_session);
        }

        public DaveDecryptor? GetDecryptor(uint ssrc)
        {
            return _decryptors.TryGetValue(ssrc, out var decryptor) ? new(decryptor) : null;
        }

        public void OnSpeaking(ulong userId, uint ssrc)
        {
            SetupKeyRatchetForUser(userId, ssrc, _latestPreparedTransitionVersion);
        }

        public void OnClientDisconnect(ulong userId)
        {
            if (_client.Cache.UserSsrcs.TryGetValue(userId, out var ssrc)
                && _decryptors.TryRemove(ssrc, out var decryptor))
                decryptor.Dispose();
        }

        public ValueTask OnSessionDescriptionAsync(ConnectionState connectionState, ushort protocolVersion)
        {
            return HandleInitAsync(connectionState, protocolVersion);
        }

        public ValueTask OnPrepareTransitionAsync(ConnectionState connectionState, ushort transitionId, ushort protocolVersion)
        {
            PrepareRatchets(transitionId, protocolVersion);
            if (transitionId is not InitTransitionId)
            {
                SetDecryptorsPassthroughMode(protocolVersion is DisabledVersion);
                return SendTransitionReadyAsync(connectionState, transitionId);
            }

            return default;
        }

        public void OnExecuteTransition(ushort transitionId)
        {
            HandleExecuteTransition(transitionId);
        }

        public ValueTask OnPrepareEpoch(ConnectionState connectionState, int epoch, ushort protocolVersion)
        {
            if (epoch is MlsNewGroupExpectedEpoch)
            {
                InitSession(protocolVersion);
                return SendMlsKeyPackageAsync(connectionState);
            }

            return default;
        }

        public void OnMlsExternalSender(ReadOnlySpan<byte> externalSender)
        {
            SessionSetExternalSender(_session, externalSender, (nuint)externalSender.Length);
        }

        public ValueTask OnMlsProposalsAsync(ConnectionState connectionState, ReadOnlySpan<byte> proposals)
        {
            var recognizedUserIds = GetRecognizedUserIds(out var buffer);

            SessionProcessProposals(_session, proposals, (nuint)proposals.Length, recognizedUserIds, (nuint)recognizedUserIds.Length, out var commitWelcome, out var commitWelcomeLength);
            using (commitWelcome)
            {
                FreeRecognizedUserIdsBuffer(buffer);

                if (!commitWelcome.IsInvalid)
                    return SendMlsCommitWelcomeAsync(connectionState, commitWelcome.AsSpan((int)commitWelcomeLength));

                return default;
            }
        }

        public ValueTask OnMlsPrepareCommitTransitionAsync(ConnectionState connectionState, ushort transitionId, ReadOnlySpan<byte> commit)
        {
            using var commitResultHandle = SessionProcessCommit(_session, commit, (nuint)commit.Length);

            if (CommitResultIsIgnored(commitResultHandle))
                return default;

            return ContinueAsync(connectionState, this, transitionId, CommitResultIsFailed(commitResultHandle));

            static async ValueTask ContinueAsync(ConnectionState connectionState, DaveSession session, ushort transitionId, bool isFailed)
            {
                var joinedGroup = !isFailed;
                if (joinedGroup)
                {
                    session.PrepareRatchets(transitionId, session.GetProtocolVersion());
                    if (transitionId is not InitTransitionId)
                        await session.SendTransitionReadyAsync(connectionState, transitionId).ConfigureAwait(false);
                }
                else
                {
                    await session.SendMlsInvalidCommitWelcomeAsync(connectionState, transitionId).ConfigureAwait(false);
                    await session.HandleInitAsync(connectionState, session.GetProtocolVersion()).ConfigureAwait(false);
                }
            }
        }

        public ValueTask OnMlsWelcomeAsync(ConnectionState connectionState, ushort transitionId, ReadOnlySpan<byte> welcome)
        {
            var recognizedUserIds = GetRecognizedUserIds(out var buffer);

            using var welcomeResult = SessionProcessWelcome(_session, welcome, (nuint)welcome.Length, recognizedUserIds, (nuint)recognizedUserIds.Length);

            FreeRecognizedUserIdsBuffer(buffer);

            return ContinueAsync(connectionState, this, transitionId, welcomeResult.IsInvalid);

            static async ValueTask ContinueAsync(ConnectionState connectionState, DaveSession session, ushort transitionId, bool isFailed)
            {
                var joinedGroup = !isFailed;
                if (joinedGroup)
                {
                    session.PrepareRatchets(transitionId, session.GetProtocolVersion());
                    if (transitionId is not InitTransitionId)
                        await session.SendTransitionReadyAsync(connectionState, transitionId).ConfigureAwait(false);
                }
                else
                {
                    await session.SendMlsInvalidCommitWelcomeAsync(connectionState, transitionId).ConfigureAwait(false);
                    await session.SendMlsKeyPackageAsync(connectionState).ConfigureAwait(false);
                }
            }
        }

        private void SetDecryptorsPassthroughMode(bool passthroughMode)
        {
            foreach (var decryptor in _decryptors.Values)
                DecryptorTransitionToPassthroughMode(decryptor, passthroughMode);
        }

        private unsafe ReadOnlySpan<nint> GetRecognizedUserIds(out nint[] pointers)
        {
            var users = _client.Cache.Users;

            int count = users.Count + 1;

            pointers = ArrayPool<nint>.Shared.Rent(count);

            var buffer = (byte*)NativeMemory.Alloc((nuint)(count * MaxSnowflakeCStringSize));

            var result = pointers.AsSpan(0, count);

            int i = 0;
            int written = 0;
            foreach (var userId in users)
                AddUserId(ref result[i++], ref written, buffer, userId);

            AddUserId(ref result[i], ref written, buffer, _client.UserId);

            return result;

            static void AddUserId(ref nint pointer, ref int written, byte* buffer, ulong userId)
            {
                var start = buffer + written;
                var writtenSpan = SnowflakeToCString(userId, new(start, MaxSnowflakeCStringSize));
                pointer = (nint)start;
                written += writtenSpan.Length;
            }
        }

        private static unsafe void FreeRecognizedUserIdsBuffer(nint[] recognizedUserIds)
        {
            var buffer = (byte*)recognizedUserIds[0];
            NativeMemory.Free(buffer);
            ArrayPool<nint>.Shared.Return(recognizedUserIds);
        }

        private ValueTask HandleInitAsync(ConnectionState connectionState, ushort protocolVersion)
        {
            InitSession(protocolVersion);

            if (protocolVersion > DisabledVersion)
                return SendMlsKeyPackageAsync(connectionState);
            else
            {
                PrepareRatchets(InitTransitionId, protocolVersion);
                HandleExecuteTransition(InitTransitionId);
            }

            return default;
        }

        private void HandleExecuteTransition(ushort transitionId)
        {
            if (!_transitions.Remove(transitionId, out var protocolVersion))
                return;

            if (protocolVersion is DisabledVersion)
                SessionReset(_session);

            SetupEncryptionKeyRatchet(protocolVersion);
        }

        private void PrepareRatchets(ushort transitionId, ushort protocolVersion)
        {
            foreach (var pair in _client.Cache.UserSsrcs)
                SetupKeyRatchetForUser(pair.Key, pair.Value, protocolVersion);

            if (transitionId is InitTransitionId)
                SetupEncryptionKeyRatchet(protocolVersion);
            else
                _transitions[transitionId] = protocolVersion;

            _latestPreparedTransitionVersion = protocolVersion;
        }

        private void SetupKeyRatchetForUser(ulong userId, uint ssrc, ushort protocolVersion)
        {
            using var keyRatchet = GetUserKeyRatchet(userId, protocolVersion);

            var decryptor = _decryptors.GetOrAdd(ssrc, ssrc =>
            {
                var decryptor = DecryptorCreate();
                DecryptorTransitionToPassthroughMode(decryptor, protocolVersion is DisabledVersion);
                return decryptor;
            });

            if (keyRatchet is not null)
                DecryptorTransitionToKeyRatchet(decryptor, keyRatchet);
        }

        private void SetupEncryptionKeyRatchet(ushort protocolVersion)
        {
            using var keyRatchet = GetUserKeyRatchet(_client.UserId, protocolVersion);
            if (keyRatchet is not null)
                EncryptorSetKeyRatchet(_encryptor, keyRatchet);
        }

        [SkipLocalsInit]
        private KeyRatchetHandle? GetUserKeyRatchet(ulong userId, ushort protocolVersion)
        {
            if (protocolVersion is DisabledVersion)
                return null;

            Span<byte> byteUserId = stackalloc byte[MaxSnowflakeCStringSize];
            byteUserId = SnowflakeToCString(userId, byteUserId);
            return SessionGetKeyRatchet(_session, byteUserId);
        }

        private async ValueTask SendMlsKeyPackageAsync(ConnectionState connectionState)
        {
            byte[] payload;
            int payloadLength;
            unsafe
            {
                SessionGetMarshalledKeyPackage(_session, out var keyPackage, out var length);
                using (keyPackage)
                {
                    payloadLength = (int)length + 1;

                    payload = ArrayPool<byte>.Shared.Rent(payloadLength);

                    keyPackage.AsSpan((int)length).CopyTo(payload.AsSpan(1));
                }
            }

            payload[0] = (byte)VoiceOpcode.DaveMlsKeyPackage;

            try
            {
                await _client.SendConnectionPayloadAsync(connectionState, payload.AsMemory(0, payloadLength), _client._internalBinaryPayloadProperties).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(payload);
            }
        }

        private ValueTask SendTransitionReadyAsync(ConnectionState connectionState, ushort transitionId)
        {
            VoicePayloadProperties<DaveTransitionReadyProperties> payload = new(VoiceOpcode.DaveTransitionReady, new(transitionId));

            return _client.SendConnectionObjectAsync(connectionState, payload, Serialization.Default.VoicePayloadPropertiesDaveTransitionReadyProperties, _client._internalTextPayloadProperties);
        }

        private ValueTask SendMlsCommitWelcomeAsync(ConnectionState connectionState, ReadOnlySpan<byte> commitWelcomeMessage)
        {
            int payloadLength = commitWelcomeMessage.Length + 1;
            var payload = ArrayPool<byte>.Shared.Rent(payloadLength);

            commitWelcomeMessage.CopyTo(payload.AsSpan(1));
            payload[0] = (byte)VoiceOpcode.DaveMlsCommitWelcome;

            return ContinueAsync(_client, connectionState, payload, payloadLength);

            static async ValueTask ContinueAsync(VoiceClient client, ConnectionState connectionState, byte[] payload, int payloadLength)
            {
                try
                {
                    await client.SendConnectionPayloadAsync(connectionState, payload.AsMemory(0, payloadLength), client._internalBinaryPayloadProperties).ConfigureAwait(false);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(payload);
                }
            }
        }

        private ValueTask SendMlsInvalidCommitWelcomeAsync(ConnectionState connectionState, ushort transitionId)
        {
            VoicePayloadProperties<DaveMlsInvalidCommitWelcomeProperties> payload = new(VoiceOpcode.DaveMlsInvalidCommitWelcome, new(transitionId));

            return _client.SendConnectionObjectAsync(connectionState, payload, Serialization.Default.VoicePayloadPropertiesDaveMlsInvalidCommitWelcomeProperties, _client._internalTextPayloadProperties);
        }

        [SkipLocalsInit]
        private void InitSession(ushort protocolVersion)
        {
            Span<byte> userId = stackalloc byte[MaxSnowflakeCStringSize];
            userId = SnowflakeToCString(_client.UserId, userId);
            SessionInit(_session, protocolVersion, _client.ChannelId, userId);
        }

        private static Span<byte> SnowflakeToCString(ulong snowflake, Span<byte> buffer)
        {
            if (!snowflake.TryFormat(buffer, out int bytesWritten))
                ThrowFailedToFormatSnowflake();

            buffer[bytesWritten] = 0;

            return buffer[..(bytesWritten + 1)];
        }

        [DoesNotReturn]
        [StackTraceHidden]
        private static void ThrowFailedToFormatSnowflake()
        {
            throw new InvalidOperationException("Failed to format snowflake.");
        }

        public void Dispose()
        {
            _session.Dispose();
            _encryptor.Dispose();
            foreach (var decryptor in _decryptors.Values)
                decryptor.Dispose();
        }
    }
}
