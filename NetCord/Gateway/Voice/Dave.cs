using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

public class DaveEncryptorException(string? message) : Exception(message)
{
    internal DaveEncryptorException(Dave.EncryptorResultCode result) : this($"Dave encryptor error: {result}")
    {
    }
}

public class DaveDecryptorException(string? message) : Exception(message)
{
    internal DaveDecryptorException(Dave.DecryptorResultCode result) : this($"Dave decryptor error: {result}")
    {
    }
}

internal static unsafe partial class Dave
{
    private const string DllName = "libdave";

    public const int InitTransitionId = 0;

    public const int DisabledVersion = 0;

    public class SessionHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            SessionDestroy(handle);
            return true;
        }
    }

    public class CommitResultHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            CommitResultDestroy(handle);
            return true;
        }
    }

    public class WelcomeResultHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            WelcomeResultDestroy(handle);
            return true;
        }
    }

    public class KeyRatchetHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            KeyRatchetDestroy(handle);
            return true;
        }
    }

    public class EncryptorHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            EncryptorDestroy(handle);
            return true;
        }
    }

    public class DecryptorHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            DecryptorDestroy(handle);
            return true;
        }
    }

    public enum CodecType
    {
        Unknown = 0,
        Opus = 1,
        Vp8 = 2,
        Vp9 = 3,
        H264 = 4,
        H265 = 5,
        Av1 = 6,
    }

    public enum MediaType
    {
        Audio = 0,
        Video = 1,
    }

    public enum EncryptorResultCode
    {
        Success = 0,
        EncryptionFailure = 1,
        MissingKeyRatchet = 2,
        MissingCryptor = 3,
        TooManyAttempts = 4,
    }

    public enum DecryptorResultCode
    {
        Success = 0,
        DecryptionFailure = 1,
        MissingKeyRatchet = 2,
        InvalidNonce = 3,
        MissingCryptor = 4,
    }

    public enum LoggingSeverity
    {
        Verbose = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        None = 4,
    }

#pragma warning disable CS0649
    public struct EncryptorStats
    {
        public ulong PassthroughCount;
        public ulong EncryptSuccessCount;
        public ulong EncryptFailureCount;
        public ulong EncryptDuration;
        public ulong EncryptAttempts;
        public ulong EncryptMaxAttempts;
        public ulong EncryptMissingKeyCount;
    }

    public struct DecryptorStats
    {
        public ulong PassthroughCount;
        public ulong DecryptSuccessCount;
        public ulong DecryptFailureCount;
        public ulong DecryptDuration;
        public ulong DecryptAttempts;
        public ulong DecryptMissingKeyCount;
        public ulong DecryptInvalidNonceCount;
    }
#pragma warning restore CS0649

    [LibraryImport(DllName, EntryPoint = "daveMaxSupportedProtocolVersion")]
    public static partial ushort MaxSupportedProtocolVersion();

    [LibraryImport(DllName, EntryPoint = "daveSessionCreate")]
    public static partial SessionHandle SessionCreate(void* context, ReadOnlySpan<byte> authSessionId, delegate*<byte*, byte*, void*, void> mlsFailureCallback, void* userData);

    [LibraryImport(DllName, EntryPoint = "daveSessionDestroy")]
    public static partial void SessionDestroy(nint session);

    [LibraryImport(DllName, EntryPoint = "daveSessionInit")]
    public static partial void SessionInit(SessionHandle session, ushort version, ulong groupId, ReadOnlySpan<byte> selfUserId);

    [LibraryImport(DllName, EntryPoint = "daveSessionReset")]
    public static partial void SessionReset(SessionHandle session);

    [LibraryImport(DllName, EntryPoint = "daveSessionSetProtocolVersion")]
    public static partial void SessionSetProtocolVersion(SessionHandle session, ushort version);

    [LibraryImport(DllName, EntryPoint = "daveSessionGetProtocolVersion")]
    public static partial ushort SessionGetProtocolVersion(SessionHandle session);

    [LibraryImport(DllName, EntryPoint = "daveSessionGetLastEpochAuthenticator")]
    public static partial void SessionGetLastEpochAuthenticator(SessionHandle session, byte** authenticator, nuint* length);

    [LibraryImport(DllName, EntryPoint = "daveSessionSetExternalSender")]
    public static partial void SessionSetExternalSender(SessionHandle session, byte* externalSender, nuint length);

    [LibraryImport(DllName, EntryPoint = "daveSessionProcessProposals")]
    public static partial void SessionProcessProposals(SessionHandle session, byte* proposals, nuint length, ReadOnlySpan<nint> recognizedUserIds, nuint recognizedUserIdsLength, out byte* commitWelcomeBytes, out nuint commitWelcomeBytesLength);

    [LibraryImport(DllName, EntryPoint = "daveSessionProcessCommit")]
    public static partial CommitResultHandle SessionProcessCommit(SessionHandle session, ReadOnlySpan<byte> commit, nuint length);

    [LibraryImport(DllName, EntryPoint = "daveSessionProcessWelcome")]
    public static partial WelcomeResultHandle SessionProcessWelcome(SessionHandle session, ReadOnlySpan<byte> welcome, nuint length, ReadOnlySpan<nint> recognizedUserIds, nuint recognizedUserIdsLength);

    [LibraryImport(DllName, EntryPoint = "daveSessionGetMarshalledKeyPackage")]
    public static partial void SessionGetMarshalledKeyPackage(SessionHandle session, out byte* keyPackage, out nuint length);

    [LibraryImport(DllName, EntryPoint = "daveSessionGetKeyRatchet", StringMarshalling = StringMarshalling.Utf8)]
    public static partial KeyRatchetHandle SessionGetKeyRatchet(SessionHandle session, ReadOnlySpan<byte> userId);

    [LibraryImport(DllName, EntryPoint = "daveSessionGetPairwiseFingerprint", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void SessionGetPairwiseFingerprint(SessionHandle session, ushort version, ReadOnlySpan<byte> userId, delegate*<byte*, nuint, void*, void> pairwiseFingerprintCallback, void* userData);

    [LibraryImport(DllName, EntryPoint = "daveKeyRatchetDestroy")]
    public static partial void KeyRatchetDestroy(nint keyRatchet);

    [LibraryImport(DllName, EntryPoint = "daveCommitResultIsFailed")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool CommitResultIsFailed(CommitResultHandle commitResultHandle);

    [LibraryImport(DllName, EntryPoint = "daveCommitResultIsIgnored")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool CommitResultIsIgnored(CommitResultHandle commitResultHandle);

    [LibraryImport(DllName, EntryPoint = "daveCommitResultGetRosterMemberIds")]
    public static partial void CommitResultGetRosterMemberIds(CommitResultHandle commitResultHandle, ulong** rosterIds, nuint* rosterIdsLength);

    [LibraryImport(DllName, EntryPoint = "daveCommitResultGetRosterMemberSignature")]
    public static partial void CommitResultGetRosterMemberSignature(CommitResultHandle commitResultHandle, ulong rosterId, byte** signature, nuint* signatureLength);

    [LibraryImport(DllName, EntryPoint = "daveCommitResultDestroy")]
    public static partial void CommitResultDestroy(nint commitResultHandle);

    [LibraryImport(DllName, EntryPoint = "daveWelcomeResultGetRosterMemberIds")]
    public static partial void WelcomeResultGetRosterMemberIds(WelcomeResultHandle welcomeResultHandle, ulong** rosterIds, nuint* rosterIdsLength);

    [LibraryImport(DllName, EntryPoint = "daveWelcomeResultGetRosterMemberSignature")]
    public static partial void WelcomeResultGetRosterMemberSignature(WelcomeResultHandle welcomeResultHandle, ulong rosterId, byte** signature, nuint* signatureLength);

    [LibraryImport(DllName, EntryPoint = "daveWelcomeResultDestroy")]
    public static partial void WelcomeResultDestroy(nint welcomeResultHandle);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorCreate")]
    public static partial EncryptorHandle EncryptorCreate();

    [LibraryImport(DllName, EntryPoint = "daveEncryptorDestroy")]
    public static partial void EncryptorDestroy(nint encryptor);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorSetKeyRatchet")]
    public static partial void EncryptorSetKeyRatchet(EncryptorHandle encryptor, KeyRatchetHandle keyRatchet);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorSetPassthroughMode")]
    public static partial void EncryptorSetPassthroughMode(EncryptorHandle encryptor, [MarshalAs(UnmanagedType.U1)] bool passthroughMode);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorAssignSsrcToCodec")]
    public static partial void EncryptorAssignSsrcToCodec(EncryptorHandle encryptor, uint ssrc, CodecType codecType);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorGetProtocolVersion")]
    public static partial ushort EncryptorGetProtocolVersion(EncryptorHandle encryptor);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorGetMaxCiphertextByteSize")]
    public static partial nuint EncryptorGetMaxCiphertextByteSize(EncryptorHandle encryptor, MediaType mediaType, nuint frameSize);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorEncrypt")]
    public static partial EncryptorResultCode EncryptorEncrypt(EncryptorHandle encryptor, MediaType mediaType, uint ssrc, byte* frame, nuint frameLength, byte* encryptedFrame, nuint encryptedFrameCapacity, out nuint bytesWritten);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorSetProtocolVersionChangedCallback")]
    public static partial void EncryptorSetProtocolVersionChangedCallback(EncryptorHandle encryptor, delegate*<void*, void> encryptorProtocolVersionChangedCallback, void* userData);

    [LibraryImport(DllName, EntryPoint = "daveEncryptorGetStats")]
    public static partial void EncryptorGetStats(EncryptorHandle encryptor, MediaType mediaType, out EncryptorStats stats);

    [LibraryImport(DllName, EntryPoint = "daveDecryptorCreate")]
    public static partial DecryptorHandle DecryptorCreate();

    [LibraryImport(DllName, EntryPoint = "daveDecryptorDestroy")]
    public static partial void DecryptorDestroy(nint decryptor);

    [LibraryImport(DllName, EntryPoint = "daveDecryptorTransitionToKeyRatchet")]
    public static partial void DecryptorTransitionToKeyRatchet(DecryptorHandle decryptor, KeyRatchetHandle keyRatchet);

    [LibraryImport(DllName, EntryPoint = "daveDecryptorTransitionToPassthroughMode")]
    public static partial void DecryptorTransitionToPassthroughMode(DecryptorHandle decryptor, [MarshalAs(UnmanagedType.U1)] bool passthroughMode);

    [LibraryImport(DllName, EntryPoint = "daveDecryptorDecrypt")]
    public static partial DecryptorResultCode DecryptorDecrypt(DecryptorHandle decryptor, MediaType mediaType, byte* encryptedFrame, nuint encryptedFrameLength, byte* frame, nuint frameCapacity, out nuint bytesWritten);

    [LibraryImport(DllName, EntryPoint = "daveDecryptorGetMaxPlaintextByteSize")]
    public static partial nuint DecryptorGetMaxPlaintextByteSize(DecryptorHandle decryptor, MediaType mediaType, nuint encryptedFrameSize);

    [LibraryImport(DllName, EntryPoint = "daveDecryptorGetStats")]
    public static partial void DecryptorGetStats(DecryptorHandle decryptor, MediaType mediaType, DecryptorStats* stats);

    [LibraryImport(DllName, EntryPoint = "daveSetLogSinkCallback")]
    public static partial void SetLogSinkCallback(delegate*<LoggingSeverity, byte*, int, byte*, void> callback);
}
