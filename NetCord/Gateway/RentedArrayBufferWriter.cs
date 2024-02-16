using System.Buffers;

namespace NetCord.Gateway;

internal class RentedArrayBufferWriter<T>(int minimumInitialCapacity) : IBufferWriter<T>, IDisposable
{
    private int _index;
    private T[] _buffer = ArrayPool<T>.Shared.Rent(minimumInitialCapacity);

    public ReadOnlyMemory<T> WrittenMemory => _buffer.AsMemory(0, _index);

    public void Advance(int count)
    {
        _index += count;
    }

    public void Clear()
    {
        _index = 0;
    }

    public Memory<T> GetMemory(int sizeHint = 1)
    {
        ResizeBuffer(sizeHint);
        return _buffer.AsMemory(_index);
    }

    public Span<T> GetSpan(int sizeHint = 1)
    {
        ResizeBuffer(sizeHint);
        return _buffer.AsSpan(_index);
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_buffer);
    }

    private void ResizeBuffer(int sizeHint)
    {
        var buffer = _buffer;
        var index = _index;
        var sum = index + sizeHint;
        if (buffer.Length < sum)
        {
            var pool = ArrayPool<T>.Shared;
            var newBuffer = pool.Rent(sum);
            Array.Copy(buffer, newBuffer, index);
            _buffer = newBuffer;
            pool.Return(buffer);
        }
    }
}
