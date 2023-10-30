using System.ComponentModel;
using System.IO.Compression;

namespace NetCord.Gateway.Compression;

public class ZLibGatewayCompression : IGatewayCompression
{
    private const int DefaultBufferSize = 8192;

    private readonly ReadOnlyMemoryStream _memoryStream;
    private readonly RentedArrayBufferWriter<byte> _writer;
    private ZLibStream? _zLibStream;

    public ZLibGatewayCompression()
    {
        _memoryStream = new();
        _writer = new(DefaultBufferSize);
    }

    public string Name => "zlib-stream";

    public ReadOnlyMemory<byte> Decompress(ReadOnlyMemory<byte> payload)
    {
        _memoryStream.SetMemory(payload);

        var writer = _writer;
        var zLibStream = _zLibStream!;
        int bytesRead;
        while ((bytesRead = zLibStream.Read(writer.GetSpan())) != 0)
            writer.Advance(bytesRead);

        var result = writer.WrittenMemory;
        writer.Clear();
        return result;
    }

    public void Initialize()
    {
        _zLibStream?.Dispose();
        _zLibStream = new(_memoryStream, CompressionMode.Decompress, true);
    }

    public void Dispose()
    {
        _memoryStream.Dispose();
        _zLibStream?.Dispose();
        _writer.Dispose();
    }

    private class ReadOnlyMemoryStream : Stream
    {
        private ReadOnlyMemory<byte> _memory;
        private int _position;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _memory.Length;

        public override long Position { get => _position; set => _position = checked((int)value); }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));

        public override int Read(Span<byte> buffer)
        {
            var span = _memory.Span;
            var position = _position;
            var length = Math.Min(span.Length - position, buffer.Length);
            span.Slice(position, length).CopyTo(buffer);
            _position += length;
            return length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return origin switch
            {
                SeekOrigin.Begin => Position = offset,
                SeekOrigin.Current => Position += offset,
                SeekOrigin.End => Position = Length - offset,
                _ => throw new InvalidEnumArgumentException(nameof(origin), (int)origin, typeof(SeekOrigin)),
            };
        }

        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public void SetMemory(ReadOnlyMemory<byte> memory)
        {
            _memory = memory;
            _position = 0;
        }
    }
}
