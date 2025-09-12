using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace NetCord.Gateway.Voice;

internal unsafe static partial class Libdavec
{
    private const string DllName = "libdavec";

    public const int InitTransitionId = 0;

    public const int DisabledVersion = 0;

    public enum MediaType : byte
    {
        Audio = 0,
        Video = 1,
    }

    public enum CodecType : byte
    {
        Unknown = 0,
        Opus = 1,
        Vp8 = 2,
        Vp9 = 3,
        H264 = 4,
        H265 = 5,
        Av1 = 6,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Buffer : IDisposable
    {
        public byte* Data;
        public nuint Length;

        public Buffer(byte* data, nuint length)
        {
            Data = data;
            Length = length;
        }

        public Buffer(byte* data, int length)
        {
            Data = data;
            Length = (nuint)length;
        }

        public readonly Span<byte> AsSpan()
        {
            return new(Data, (int)Length);
        }

        public readonly void Dispose()
        {
            BufferFree(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HashRatchet : IDisposable
    {
        public ushort CipherSuite;
        public Buffer BaseSecret;

        public readonly void Dispose()
        {
            BaseSecret.Dispose();
        }
    }

    [NativeMarshalling(typeof(CommitProcessingResultMarshaller))]
    [StructLayout(LayoutKind.Sequential)]
    public struct CommitProcessingResult : IDisposable
    {
        public bool Failed;
        public bool Ignored;
        public RosterMapHandle RosterUpdate;

        public readonly void Dispose()
        {
            RosterUpdate.Dispose();
        }
    }

    [CustomMarshaller(typeof(CommitProcessingResult), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToUnmanagedIn))]
    [CustomMarshaller(typeof(CommitProcessingResult), MarshalMode.ManagedToUnmanagedOut, typeof(ManagedToUnmanagedOut))]
    internal static unsafe class CommitProcessingResultMarshaller
    {
        public struct ManagedToUnmanagedIn
        {
            private bool _addRefd;
            private CommitProcessingResult _value;

            public void FromManaged(CommitProcessingResult value)
            {
                _value = value;
                value.RosterUpdate.DangerousAddRef(ref _addRefd);
            }

            public InternalCommitProcessingResult ToUnmanaged()
            {
                var value = _value;
                return new()
                {
                    Failed = value.Failed,
                    Ignored = value.Ignored,
                    RosterUpdate = value.RosterUpdate.DangerousGetHandle(),
                };
            }

            public readonly void Free()
            {
                if (_addRefd)
                    _value.RosterUpdate.DangerousRelease();
            }
        }

        public struct ManagedToUnmanagedOut
        {
            private bool _initialized;
            private CommitProcessingResult _value;

            public void FromUnmanaged(InternalCommitProcessingResult value)
            {
                _initialized = true;

                RosterMapHandle rosterUpdate = new();
                Marshal.InitHandle(rosterUpdate, value.RosterUpdate);

                _value = new()
                {
                    Failed = value.Failed,
                    Ignored = value.Ignored,
                    RosterUpdate = rosterUpdate,
                };
            }

            public readonly CommitProcessingResult ToManaged() => _value;

            public readonly void Free()
            {
                if (!_initialized)
                    _value.RosterUpdate.Dispose();
            }
        }

        public struct InternalCommitProcessingResult
        {
            public bool Failed;
            public bool Ignored;
            public nint RosterUpdate;
        }
    }

    // Delegates
    public delegate void MlsFailureCallback(byte* context, byte* authSessionId);

    public delegate void ProtocolVersionChangedCallback();

    // Session methods
    [LibraryImport(DllName, EntryPoint = "dave_max_supported_protocol_version")]
    public static partial ushort MaxSupportedProtocolVersion();

    [LibraryImport(DllName, EntryPoint = "dave_session_create")]
    public static partial SessionHandle SessionCreate(ReadOnlySpan<byte> context, ReadOnlySpan<byte> authSessionId, MlsFailureCallback mlsFailureCallback);

    [LibraryImport(DllName, EntryPoint = "dave_session_free")]
    public static partial void SessionFree(nint session);

    [LibraryImport(DllName, EntryPoint = "dave_session_init")]
    public static partial void SessionInit(SessionHandle session, ushort protocolVersion, ulong groupId, ReadOnlySpan<byte> selfUserId, TransientPrivateKeyHandle transientKey);

    [LibraryImport(DllName, EntryPoint = "dave_session_reset")]
    public static partial void SessionReset(SessionHandle session);

    [LibraryImport(DllName, EntryPoint = "dave_session_set_protocol_version")]
    public static partial void SessionSetProtocolVersion(SessionHandle session, ushort protocolVersion);

    [LibraryImport(DllName, EntryPoint = "dave_session_get_protocol_version")]
    public static partial ushort SessionGetProtocolVersion(SessionHandle session);

    [LibraryImport(DllName, EntryPoint = "dave_session_get_last_epoch_authenticator")]
    public static partial Buffer SessionGetLastEpochAuthenticator(SessionHandle session);

    [LibraryImport(DllName, EntryPoint = "dave_session_set_external_sender")]
    public static partial void SessionSetExternalSender(SessionHandle session, Buffer marshalledExternalSender);

    [LibraryImport(DllName, EntryPoint = "dave_session_process_proposals")]
    public static partial Buffer SessionProcessProposals(SessionHandle session, Buffer proposals, ReadOnlySpan<nint> recognizedUserIds, nuint recognizedUserIdsCount);

    [LibraryImport(DllName, EntryPoint = "dave_session_process_commit")]
    public static partial CommitProcessingResult SessionProcessCommit(SessionHandle session, Buffer commit);

    [LibraryImport(DllName, EntryPoint = "dave_session_process_welcome")]
    public static partial RosterMapHandle SessionProcessWelcome(SessionHandle session, Buffer welcome, ReadOnlySpan<nint> recognizedUserIds, nuint recognizedUserIdsCount);

    [LibraryImport(DllName, EntryPoint = "dave_session_get_marshalled_key_package")]
    public static partial Buffer SessionGetMarshalledKeyPackage(SessionHandle session);

    [LibraryImport(DllName, EntryPoint = "dave_session_get_key_ratchet")]
    public static partial HashRatchet SessionGetKeyRatchet(SessionHandle session, ReadOnlySpan<byte> userId);

    // Encryptor methods
    [LibraryImport(DllName, EntryPoint = "dave_encryptor_create")]
    public static partial EncryptorHandle EncryptorCreate();

    [LibraryImport(DllName, EntryPoint = "dave_encryptor_free")]
    public static partial void EncryptorFree(nint encryptor);

    [LibraryImport(DllName, EntryPoint = "dave_encryptor_set_key_ratchet")]
    public static partial void EncryptorSetKeyRatchet(EncryptorHandle encryptor, HashRatchet keyRatchet);

    [LibraryImport(DllName, EntryPoint = "dave_encryptor_set_passthrough_mode")]
    public static partial void EncryptorSetPassthroughMode(EncryptorHandle encryptor, [MarshalAs(UnmanagedType.U1)] bool passthroughMode);

    [LibraryImport(DllName, EntryPoint = "dave_encryptor_assign_ssrc_to_codec")]
    public static partial void EncryptorAssignSsrcToCodec(EncryptorHandle encryptor, uint ssrc, CodecType codecType);

    [LibraryImport(DllName, EntryPoint = "dave_encryptor_get_protocol_version")]
    public static partial ushort EncryptorGetProtocolVersion(EncryptorHandle encryptor);

    [LibraryImport(DllName, EntryPoint = "dave_encryptor_get_max_ciphertext_byte_size")]
    public static partial nuint EncryptorGetMaxCiphertextByteSize(EncryptorHandle encryptor, MediaType mediaType, nuint frameSize);

    [LibraryImport(DllName, EntryPoint = "dave_encryptor_encrypt")]
    public static partial nuint EncryptorEncrypt(EncryptorHandle encryptor, MediaType mediaType, uint ssrc, Buffer frame, Buffer encryptedFrame);

    [LibraryImport(DllName, EntryPoint = "dave_encryptor_set_protocol_version_changed_callback")]
    public static partial void EncryptorSetProtocolVersionChangedCallback(EncryptorHandle encryptor, ProtocolVersionChangedCallback callback);

    // Decryptor methods
    [LibraryImport(DllName, EntryPoint = "dave_decryptor_create")]
    public static partial DecryptorHandle DecryptorCreate();

    [LibraryImport(DllName, EntryPoint = "dave_decryptor_free")]
    public static partial void DecryptorFree(nint decryptor);

    [LibraryImport(DllName, EntryPoint = "dave_decryptor_transition_to_key_ratchet")]
    public static partial void DecryptorTransitionToKeyRatchet(DecryptorHandle decryptor, HashRatchet keyRatchet, long transitionExpirySeconds);

    [LibraryImport(DllName, EntryPoint = "dave_decryptor_transition_to_passthrough_mode")]
    public static partial void DecryptorTransitionToPassthroughMode(DecryptorHandle decryptor, [MarshalAs(UnmanagedType.U1)] bool passthroughMode, long transitionExpirySeconds);

    [LibraryImport(DllName, EntryPoint = "dave_decryptor_decrypt")]
    public static partial nuint DecryptorDecrypt(DecryptorHandle decryptor, MediaType mediaType, Buffer encryptedFrame, Buffer frame);

    [LibraryImport(DllName, EntryPoint = "dave_decryptor_get_max_plaintext_byte_size")]
    public static partial nuint DecryptorGetMaxPlaintextByteSize(DecryptorHandle decryptor, MediaType mediaType, nuint encryptedFrameSize);

    // Transient key methods
    [LibraryImport(DllName, EntryPoint = "dave_transient_private_key_generate")]
    public static partial TransientPrivateKeyHandle TransientPrivateKeyGenerate(ushort protocolVersion);

    [LibraryImport(DllName, EntryPoint = "dave_transient_private_key_free")]
    public static partial void TransientPrivateKeyFree(nint key);

    // Roster map methods
    [LibraryImport(DllName, EntryPoint = "dave_roster_map_find")]
    public static partial Buffer RosterMapFind(RosterMapHandle rosterMap, ulong key);

    [LibraryImport(DllName, EntryPoint = "dave_roster_map_free")]
    public static partial void RosterMapFree(nint rosterMap);

    // Memory management methods
    [LibraryImport(DllName, EntryPoint = "dave_buffer_free")]
    public static partial void BufferFree(Buffer buffer);

    [LibraryImport(DllName, EntryPoint = "dave_commit_processing_result_free")]
    public static partial void CommitProcessingResultFree(CommitProcessingResult result);

    [LibraryImport(DllName, EntryPoint = "dave_hash_ratchet_free")]
    public static partial void HashRatchetFree(HashRatchet ratchet);

    // Safe handle classes
    public class SessionHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            SessionFree(handle);
            return true;
        }
    }

    public class EncryptorHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            EncryptorFree(handle);
            return true;
        }
    }

    public class DecryptorHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            DecryptorFree(handle);
            return true;
        }
    }

    public class TransientPrivateKeyHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            TransientPrivateKeyFree(handle);
            return true;
        }
    }

    public class RosterMapHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            RosterMapFree(handle);
            return true;
        }
    }
}
