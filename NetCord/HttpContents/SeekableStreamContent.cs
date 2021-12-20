
using System.Net;

namespace NetCord
{
    internal class SeekableStreamContent : HttpContent
    {
        private readonly Stream _stream;
        private readonly long _start;

        public SeekableStreamContent(Stream stream)
        {
            if (!stream.CanSeek)
                throw new InvalidOperationException("This stream does not support seeking");

            _stream = stream;
            _start = _stream.Position;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            if (_stream.Position != _start)
                _stream.Position = _start;

            return _stream.CopyToAsync(stream);
        }
        
        protected override bool TryComputeLength(out long length)
        {
            if (_stream.CanSeek)
            {
                length = _stream.Length - _start;
                return true;
            }
            else
            {
                length = 0;
                return false;
            }
        }
    }
}