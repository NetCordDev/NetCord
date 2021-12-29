using System.Collections;
using System.Runtime.CompilerServices;

namespace NetCord.Commands;

internal class SortedList<T> : ICollection<T>, IReadOnlyCollection<T>
{
    private T[] _items;
    private readonly Comparison<T> _comparison;
    private int _size;

    public SortedList(Comparison<T> comparison)
    {
        _items = Array.Empty<T>();
        _size = 0;
        _comparison = comparison;
    }

    public int Count => _size;
    bool ICollection<T>.IsReadOnly => false;
    void ICollection<T>.Add(T item)
    {
        for (int i = 1; i < _size; i++)
        {

        }
    }
    void ICollection<T>.Clear()
    {
        _items = Array.Empty<T>();
    }
    bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }
    bool ICollection<T>.Remove(T item)
    {
        var index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }
    public void RemoveAt(int index)
    {
        if (index >= _size)
            throw new ArgumentOutOfRangeException(nameof(index));
        _size--;
        if (index < _size)
            Array.Copy(_items, index + 1, _items, index, _items.Length - index);
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            _items[index] = default!;
    }
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_items[.._size].GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _items[.._size].GetEnumerator();

    public int IndexOf(T item) => Array.IndexOf(_items, item);
}