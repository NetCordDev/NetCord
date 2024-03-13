using System.Buffers.Binary;
using System.Text;

namespace NetCord.Services.ApplicationCommands;

internal sealed class AutoTranscodingStream(Stream innerStream, Encoding outerStreamEncoding) : Stream
{
    private sealed class StartBytesStream(ReadOnlyMemory<byte> bytes, Stream stream) : Stream
    {
        private int _bytesPosition;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush() => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

        public override int Read(Span<byte> buffer)
        {
            if (_bytesPosition >= 0)
            {
                int maxLength = Math.Min(buffer.Length, bytes.Length);
                bytes.Span[_bytesPosition..maxLength].CopyTo(buffer);
                int written = maxLength - _bytesPosition;

                if ((_bytesPosition += written) == bytes.Length)
                {
                    bytes = null;
                    _bytesPosition = -1;
                }

                return stream.Read(buffer[written..]) + written;
            }

            return stream.Read(buffer);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_bytesPosition >= 0)
            {
                int maxLength = Math.Min(buffer.Length, bytes.Length);
                bytes[_bytesPosition..maxLength].CopyTo(buffer);
                int written = maxLength - _bytesPosition;

                if ((_bytesPosition += written) == bytes.Length)
                {
                    bytes = null;
                    _bytesPosition = -1;
                }

                return ReadAsyncAndAppendWritten(stream, buffer, written, cancellationToken);

                static async ValueTask<int> ReadAsyncAndAppendWritten(Stream stream, Memory<byte> buffer, int written, CancellationToken cancellationToken)
                {
                    return await stream.ReadAsync(buffer[written..], cancellationToken).ConfigureAwait(false) + written;
                }
            }

            return stream.ReadAsync(buffer, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                stream.Dispose();
        }
    }

    private Stream? _transcodingStream;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush() => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

    public override int Read(Span<byte> buffer)
    {
        if (_transcodingStream is null)
        {
            var data = new byte[4];
            int length = 0;
            do
            {
                int read = innerStream.Read(data.AsSpan(length));
                if (read == 0)
                    break;
                length += read;
            }
            while (length != 4);

            var encoding = DetectEncoding(data, out int preambleLength);

            var stream = preambleLength == 4 ? innerStream : new StartBytesStream(data.AsMemory(preambleLength), innerStream);
            _transcodingStream = Encoding.CreateTranscodingStream(stream, encoding, outerStreamEncoding);
        }

        return _transcodingStream.Read(buffer);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_transcodingStream is null)
        {
            return PrepareAndReadAsync(buffer, cancellationToken);

            async ValueTask<int> PrepareAndReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
            {
                var data = new byte[4];
                int length = 0;
                do
                {
                    int read = await innerStream.ReadAsync(data.AsMemory(length), cancellationToken).ConfigureAwait(false);
                    if (read == 0)
                        break;
                    length += read;
                }
                while (length != 4);

                var encoding = DetectEncoding(data, out int preambleLength);

                var stream = preambleLength == 4 ? innerStream : new StartBytesStream(data.AsMemory(preambleLength), innerStream);
                var transcodingStream = _transcodingStream = Encoding.CreateTranscodingStream(stream, encoding, outerStreamEncoding);

                return await transcodingStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            }
        }

        return _transcodingStream.ReadAsync(buffer, cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            (_transcodingStream is Stream transcodingStream ? transcodingStream : innerStream).Dispose();
    }

    private static Encoding DetectEncoding(Span<byte> byteBuffer, out int preambleLength)
    {
        int len = byteBuffer.Length;

        Encoding result;

        if (len < 2)
        {
            preambleLength = 0;
            result = Encoding.UTF8;
        }
        else
        {
            ushort firstTwoBytes = BinaryPrimitives.ReadUInt16LittleEndian(byteBuffer);
            if (firstTwoBytes == 0xFFFE)
            {
                // Big Endian Unicode
                preambleLength = 2;
                result = Encoding.BigEndianUnicode;
            }
            else if (firstTwoBytes == 0xFEFF)
            {
                // Little Endian Unicode, or possibly little endian UTF32
                if (len < 4 || byteBuffer[2] != 0 || byteBuffer[3] != 0)
                {
                    preambleLength = 2;
                    result = Encoding.Unicode;
                }
                else
                {
                    preambleLength = 4;
                    result = Encoding.UTF32;
                }
            }
            else if (len >= 3 && firstTwoBytes == 0xBBEF && byteBuffer[2] == 0xBF)
            {
                // UTF-8
                preambleLength = 3;
                result = Encoding.UTF8;
            }
            else if (len >= 4 && firstTwoBytes == 0 && byteBuffer[2] == 0xFE && byteBuffer[3] == 0xFF)
            {
                // Big Endian UTF32
                preambleLength = 4;
                result = new UTF32Encoding(bigEndian: true, byteOrderMark: true);
            }
            else
            {
                preambleLength = 0;
                result = Encoding.UTF8;
            }
        }

        return result;
    }
}
