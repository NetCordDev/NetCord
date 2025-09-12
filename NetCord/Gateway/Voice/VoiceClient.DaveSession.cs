using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static NetCord.Gateway.Voice.Libdavec;

namespace NetCord.Gateway.Voice;

public partial class VoiceClient
{
    internal struct DaveEncryptor(EncryptorHandle encryptor)
    {
        public readonly unsafe int Encrypt(uint ssrc, ReadOnlySpan<byte> frame, Span<byte> encryptedFrame)
        {
            fixed (byte* frameP = frame, encryptedFrameP = encryptedFrame)
                return (int)EncryptorEncrypt(encryptor, MediaType.Audio, ssrc, new(frameP, frame.Length), new(encryptedFrameP, encryptedFrame.Length));
        }

        public readonly int GetMaxCiphertextByteSize(int plaintextByteSize)
            => (int)EncryptorGetMaxCiphertextByteSize(encryptor, MediaType.Audio, (nuint)plaintextByteSize);
    }

    internal struct DaveDecryptor(DecryptorHandle decryptor)
    {
        public readonly unsafe int Decrypt(uint ssrc, ReadOnlySpan<byte> encryptedFrame, Span<byte> frame)
        {
            fixed (byte* encryptedFrameP = encryptedFrame, frameP = frame)
                return (int)DecryptorDecrypt(decryptor, MediaType.Audio, new(encryptedFrameP, encryptedFrame.Length), new(frameP, frame.Length));
        }

        public readonly int GetMaxPlaintextByteSize(int ciphertextByteSize)
            => (int)DecryptorGetMaxPlaintextByteSize(decryptor, MediaType.Audio, (nuint)ciphertextByteSize);
    }

    internal class DaveSession(VoiceClient client, MlsFailureCallback mlsFailureCallback) : IDisposable
    {
        private const int MaxSnowflakeCStringSize = 21;

        private const int MlsNewGroupExpectedEpoch = 1;

        private readonly SessionHandle _session = SessionCreate(""u8, ""u8, mlsFailureCallback);

        private readonly Dictionary<ushort, ushort> _transitions = [];

        private ushort _latestPreparedTransitionVersion;

        private readonly EncryptorHandle _encryptor = EncryptorCreate();

        private readonly ConcurrentDictionary<uint, DecryptorHandle> _decryptors = [];

        public static ushort GetMaxSupportedProtocolVersion()
        {
            return MaxSupportedProtocolVersion();
        }

        public ushort GetProtocolVersion()
        {
            return SessionGetProtocolVersion(_session);
        }

        public DaveEncryptor GetEncryptor()
        {
            return new(_encryptor);
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
            if (client.Cache.UserSsrcs.TryGetValue(userId, out var ssrc)
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
                SetDecryptorsPassthroughMode(protocolVersion is DisabledVersion, 10);
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

        public unsafe void OnMlsExternalSender(ReadOnlySpan<byte> externalSender)
        {
            fixed (byte* p = externalSender)
                SessionSetExternalSender(_session, new(p, externalSender.Length));
        }

        public unsafe ValueTask OnMlsProposalsAsync(ConnectionState connectionState, ReadOnlySpan<byte> proposals)
        {
            var recognizedUserIds = GetRecognizedUserIds(out var buffer);

            Libdavec.Buffer commitWelcome;
            fixed (byte* proposalsP = proposals)
                commitWelcome = SessionProcessProposals(_session, new(proposalsP, proposals.Length), recognizedUserIds, (nuint)recognizedUserIds.Length);

            FreeRecognizedUserIdsBuffer(buffer);

            if (commitWelcome.Data is not null)
                return SendMlsCommitWelcomeAsync(connectionState, commitWelcome);

            return default;
        }

        public ValueTask OnMlsPrepareCommitTransitionAsync(ConnectionState connectionState, ushort transitionId, ReadOnlySpan<byte> commit)
        {
            CommitProcessingResult processingResult;
            unsafe
            {
                fixed (byte* p = commit)
                    processingResult = SessionProcessCommit(_session, new(p, commit.Length));
            }

            if (processingResult.Ignored)
                return default;

            return HandleRosterUpdatedAsync(connectionState, transitionId, processingResult.RosterUpdate);
        }

        public ValueTask OnMlsWelcomeAsync(ConnectionState connectionState, ushort transitionId, ReadOnlySpan<byte> welcome)
        {
            var recognizedUserIds = GetRecognizedUserIds(out var buffer);

            RosterMapHandle rosterMap;
            unsafe
            {
                fixed (byte* p = welcome)
                    rosterMap = SessionProcessWelcome(_session, new(p, welcome.Length), recognizedUserIds, (nuint)recognizedUserIds.Length);
            }

            FreeRecognizedUserIdsBuffer(buffer);

            return HandleRosterUpdatedAsync(connectionState, transitionId, rosterMap);
        }

        private void SetDecryptorsPassthroughMode(bool infinite, int transitionExpirySeconds)
        {
            foreach (var decryptor in _decryptors.Values)
                DecryptorTransitionToPassthroughMode(decryptor, infinite, transitionExpirySeconds);
        }

        private async ValueTask HandleRosterUpdatedAsync(ConnectionState connectionState, ushort transitionId, RosterMapHandle rosterMap)
        {
            var joinedGroup = !rosterMap.IsInvalid;
            if (joinedGroup)
            {
                PrepareRatchets(transitionId, GetProtocolVersion());
                if (transitionId is not InitTransitionId)
                    await SendTransitionReadyAsync(connectionState, transitionId).ConfigureAwait(false);
            }
            else
            {
                await SendMlsInvalidCommitWelcomeAsync(connectionState, transitionId).ConfigureAwait(false);
                await SendMlsKeyPackageAsync(connectionState).ConfigureAwait(false);
            }
        }

        private unsafe ReadOnlySpan<nint> GetRecognizedUserIds(out nint[] buffer)
        {
            var users = client.Cache.Users;

            int count = users.Count + 1;

            buffer = ArrayPool<nint>.Shared.Rent(count);

            var result = buffer.AsSpan(0, count);

            int i = 0;
            foreach (var userId in users)
                AddUserId(ref result[i++], userId);

            AddUserId(ref result[i], client.UserId);

            return result;

            static void AddUserId(ref nint value, ulong userId)
            {
                value = (nint)NativeMemory.Alloc(MaxSnowflakeCStringSize);
                var span = SnowflakeToCString(userId, new((void*)value, MaxSnowflakeCStringSize));
            }
        }

        private unsafe static void FreeRecognizedUserIdsBuffer(nint[] recognizedUserIds)
        {
            int count = recognizedUserIds.Length;
            for (int i = 0; i < count; i++)
            {
                var ptr = (void*)recognizedUserIds[i];
                if (ptr is not null)
                    NativeMemory.Free(ptr);
            }
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
            foreach (var pair in client.Cache.UserSsrcs)
                SetupKeyRatchetForUser(pair.Key, pair.Value, protocolVersion);

            if (transitionId is InitTransitionId)
                SetupEncryptionKeyRatchet(protocolVersion);
            else
                _transitions[transitionId] = protocolVersion;

            _latestPreparedTransitionVersion = protocolVersion;
        }

        private void SetupKeyRatchetForUser(ulong userId, uint ssrc, ushort protocolVersion)
        {
            var result = GetUserKeyRatchet(userId, protocolVersion);

            var decryptor = _decryptors.GetOrAdd(ssrc, ssrc =>
            {
                var decryptor = DecryptorCreate();
                DecryptorTransitionToPassthroughMode(decryptor, protocolVersion is DisabledVersion, 10);
                return decryptor;
            });

            if (result is { BaseSecret.Data: not null })
                DecryptorTransitionToKeyRatchet(decryptor, result.GetValueOrDefault(), 10);
        }

        private void SetupEncryptionKeyRatchet(ushort protocolVersion)
        {
            var result = GetUserKeyRatchet(client.UserId, protocolVersion);
            if (result is { BaseSecret.Data: not null } keyRatchet)
                EncryptorSetKeyRatchet(_encryptor, keyRatchet);
        }

        private HashRatchet? GetUserKeyRatchet(ulong userId, ushort protocolVersion)
        {
            if (protocolVersion is DisabledVersion)
                return null;

            Span<byte> byteUserId = stackalloc byte[MaxSnowflakeCStringSize];
            byteUserId = SnowflakeToCString(userId, byteUserId);
            return SessionGetKeyRatchet(_session, byteUserId);
        }

        private ValueTask SendMlsKeyPackageAsync(ConnectionState connectionState)
        {
            var keyPackage = SessionGetMarshalledKeyPackage(_session);
            var payload = new byte[keyPackage.Length + 1];
            keyPackage.AsSpan().CopyTo(payload.AsSpan(1));
            payload[0] = (byte)VoiceOpcode.DaveMlsKeyPackage;

            return client.SendConnectionPayloadAsync(connectionState, payload, client._internalBinaryPayloadProperties);

            // TODO: Send keyPackage to the voice gateway using the MLS_KEY_PACKAGE (26) opcode
        }

        private ValueTask SendTransitionReadyAsync(ConnectionState connectionState, ushort transitionId)
        {
            VoicePayloadProperties<DaveTransitionReadyProperties> readyPayload = new(VoiceOpcode.DaveTransitionReady, new(transitionId));

            return client.SendConnectionPayloadAsync(connectionState, readyPayload.Serialize(Serialization.Default.VoicePayloadPropertiesDaveTransitionReadyProperties), client._internalTextPayloadProperties);

            // TODO: Send the transition ready message to the voice gateway using the DAVE_PROTOCOL_READY_FOR_TRANSITION (23) opcode
        }

        private ValueTask SendMlsCommitWelcomeAsync(ConnectionState connectionState, Libdavec.Buffer commitWelcomeMessage)
        {
            var commitWelcomeSpan = commitWelcomeMessage.AsSpan();

            var payload = new byte[commitWelcomeSpan.Length + 1];
            commitWelcomeSpan.CopyTo(payload.AsSpan(1));
            payload[0] = (byte)VoiceOpcode.DaveMlsCommitWelcome;

            return client.SendConnectionPayloadAsync(connectionState, payload, client._internalBinaryPayloadProperties);

            // TODO: Send the commit welcome message to the voice gateway using the MLS_COMMIT_WELCOME (28) opcode
        }

        private ValueTask SendMlsInvalidCommitWelcomeAsync(ConnectionState connectionState, ushort transitionId)
        {
            VoicePayloadProperties<DaveMlsInvalidCommitWelcomeProperties> invalidCommitWelcomePayload = new(VoiceOpcode.DaveMlsInvalidCommitWelcome, new(transitionId));

            return client.SendConnectionPayloadAsync(connectionState, invalidCommitWelcomePayload.Serialize(Serialization.Default.VoicePayloadPropertiesDaveMlsInvalidCommitWelcomeProperties), client._internalTextPayloadProperties);

            // TODO: Send the invalid commit welcome message to the voice gateway using the MLS_INVALID_COMMIT_WELCOME (31) opcode
        }

        [SkipLocalsInit]
        private void InitSession(ushort protocolVersion)
        {
            Span<byte> userId = stackalloc byte[MaxSnowflakeCStringSize];
            userId = SnowflakeToCString(client.UserId, userId);
            SessionInit(_session, protocolVersion, client.ChannelId, userId, new());
        }

        private static Span<byte> SnowflakeToCString(ulong snowflake, Span<byte> buffer)
        {
            if (!snowflake.TryFormat(buffer, out int bytesWritten))
                ThrowFailedToFormatSnowflake();

            buffer[bytesWritten] = 0;

            return buffer[..(bytesWritten + 1)];
        }

        [DoesNotReturn]
        private static void ThrowFailedToFormatSnowflake()
        {
            throw new InvalidOperationException("Failed to format snowflake.");
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
