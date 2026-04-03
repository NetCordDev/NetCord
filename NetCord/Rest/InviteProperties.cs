using System.Buffers;
using System.Buffers.Text;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class InviteProperties : IHttpSerializable
{
    [JsonPropertyName("max_age")]
    public int? MaxAge { get; set; }

    [JsonPropertyName("max_uses")]
    public int? MaxUses { get; set; }

    [JsonPropertyName("temporary")]
    public bool? Temporary { get; set; }

    [JsonPropertyName("unique")]
    public bool? Unique { get; set; }

    [JsonPropertyName("target_type")]
    public InviteTargetType? TargetType { get; set; }

    [JsonPropertyName("target_user_id")]
    public ulong? TargetUserId { get; set; }

    [JsonPropertyName("target_application_id")]
    public ulong? TargetApplicationId { get; set; }

    [JsonIgnore]
    public InviteTargetUsersProperties? TargetUsers { get; set; }

    [JsonPropertyName("role_ids")]
    public IEnumerable<ulong>? RoleIds { get; set; }

    HttpContent IHttpSerializable.Serialize() => Serialize();

    internal HttpContent Serialize()
    {
        JsonContent<InviteProperties> inviteContent = new(this, Serialization.Default.InviteProperties);

        if (TargetUsers is not { } targetUsers)
            return inviteContent;

        return new MultipartFormDataContent()
        {
            { inviteContent, "payload_json" },
            { targetUsers.Serialize(), "target_users_file", "target_users_file" }
        };
    }
}

[GenerateMethodsForProperties]
public partial class InviteTargetUsersProperties : IHttpSerializable
{
    private readonly Stream _stream;

    private InviteTargetUsersProperties(Stream stream)
    {
        _stream = stream;
    }

    public static InviteTargetUsersProperties FromStream(Stream stream) => new(stream);

    public static InviteTargetUsersProperties FromEnumerable(IEnumerable<ulong> userIds) => new(new UserIdsStream(userIds));

    HttpContent IHttpSerializable.Serialize() => Serialize();

    internal HttpContent Serialize() => new StreamContent(_stream);

    private sealed class UserIdsStream(IEnumerable<ulong> userIds) : Stream
    {
        private readonly IEnumerator<ulong> _enumerator = userIds.GetEnumerator();
        private byte[] _buffer = ArrayPool<byte>.Shared.Rent(22);
        private int _startPosition;
        private int _endPosition;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush() => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

        public override int Read(Span<byte> buffer)
        {
            int totalWritten = 0;

            if (_startPosition != _endPosition)
            {
                var bufferedLength = _endPosition - _startPosition;

                var length = Math.Min(bufferedLength, buffer.Length);

                _buffer.AsSpan(_startPosition, length).CopyTo(buffer);
                _startPosition += length;

                if (length != bufferedLength)
                    return length;

                totalWritten += length;
            }

            var newLine = "\r\n"u8;

            while (_enumerator.MoveNext())
            {
                var userId = _enumerator.Current;
                if (!Utf8Formatter.TryFormat(userId, buffer[totalWritten..], out var written))
                {
                    _ = Utf8Formatter.TryFormat(userId, _buffer, out var writtenToBuffer);

                    _buffer.AsSpan(0, _startPosition = written = buffer.Length - totalWritten).CopyTo(buffer[totalWritten..]);

                    totalWritten += written;

                    newLine.CopyTo(_buffer[writtenToBuffer..]);
                    _endPosition = writtenToBuffer + newLine.Length;
                    break;
                }

                totalWritten += written;

                if (!newLine.TryCopyTo(buffer[totalWritten..]))
                {
                    var remaining = buffer.Length - totalWritten;
                    if (remaining > 0)
                    {
                        newLine[..remaining].CopyTo(buffer[totalWritten..]);
                        totalWritten += remaining;
                    }

                    newLine[remaining..].CopyTo(_buffer);
                    _startPosition = 0;
                    _endPosition = newLine.Length - remaining;

                    break;
                }

                totalWritten += newLine.Length;
            }

            return totalWritten;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return new(Read(buffer.Span));
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _enumerator.Dispose();
                if (Interlocked.Exchange(ref _buffer, null!) is { } buffer)
                    ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
