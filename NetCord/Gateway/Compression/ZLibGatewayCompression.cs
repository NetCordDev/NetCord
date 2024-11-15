using System.IO.Compression;

namespace NetCord.Gateway.Compression;

public sealed class ZLibGatewayCompression : IGatewayCompression
{
    private const int DefaultBufferSize = 8192;

    private readonly RentedArrayBufferWriter<byte> _writer;
    private readonly MutableUnmanagedMemoryStream _memoryStream;
    private ZLibStream? _zLibStream;

    public ZLibGatewayCompression()
    {
        _writer = new(DefaultBufferSize);
        _memoryStream = new();
    }

    public string Name => "zlib-stream";

    public unsafe ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> payload)
    {
        var writer = _writer;
        var zLibStream = _zLibStream!;

        writer.Clear();

        fixed (byte* payloadPtr = payload)
        {
            _memoryStream.SetMemory(payloadPtr, payload.Length);

            int bytesRead;
            while ((bytesRead = zLibStream.Read(writer.GetSpan())) != 0)
                writer.Advance(bytesRead);
        }

        return writer.WrittenSpan;
    }

    public void Initialize()
    {
        _zLibStream?.Dispose();
        _zLibStream = new(_memoryStream, CompressionMode.Decompress, true);
    }

    public void Dispose()
    {
        _zLibStream?.Dispose();
        _writer.Dispose();
    }

    private unsafe class MutableUnmanagedMemoryStream : Stream
    {
        private byte* _start;
        private byte* _end;

        public void SetMemory(byte* pointer, int length)
        {
            _start = pointer;
            _end = pointer + length;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));

        public override int Read(Span<byte> buffer)
        {
            var start = _start;
            var length = Math.Min((int)(_end - start), buffer.Length);
            new ReadOnlySpan<byte>(start, length).CopyTo(buffer);
            _start += length;
            return length;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
