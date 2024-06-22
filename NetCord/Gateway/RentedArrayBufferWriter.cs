using System.Buffers;

namespace NetCord.Gateway;

#pragma warning disable IDE0032 // Use auto property

internal sealed class RentedArrayBufferWriter<T>(int minimumInitialCapacity) : IBufferWriter<T>, IDisposable
{
    private int _index;

    private T[] _buffer = ArrayPool<T>.Shared.Rent(minimumInitialCapacity);

    public int WrittenCount => _index;

    public ReadOnlyMemory<T> WrittenMemory => _buffer.AsMemory(0, _index);

    public ReadOnlySpan<T> WrittenSpan => _buffer.AsSpan(0, _index);

    public int FreeCapacity => _buffer.Length - _index;

    public void Advance(int count)
    {
        _index += count;
    }

    public void Clear()
    {
        _index = 0;
    }

    public Memory<T> GetMemory(int sizeHint = 0)
    {
        ResizeBuffer(sizeHint);
        return _buffer.AsMemory(_index);
    }

    public Span<T> GetSpan(int sizeHint = 0)
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
        if (sizeHint is 0)
            sizeHint = 1;

        var buffer = _buffer;
        int index = _index;
        int sum = index + sizeHint;
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
