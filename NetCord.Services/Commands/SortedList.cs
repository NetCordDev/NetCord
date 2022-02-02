using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Commands;

internal class SortedList<T> : ICollection<T>, IReadOnlyCollection<T>
{
    private const int DefaultCapacity = 4;
    private T[] _items;
    private readonly Comparison<T> _comparison;
    private int _size;

    public T this[int index] => _items[index];

    public int Capacity
    {
        get => _items.Length;
        set
        {
            if (value < _size)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (value != _items.Length)
                if (value > 0)
                {
                    var newItems = new T[value];
                    if (_size > 0)
                        Array.Copy(_items, newItems, _size);
                    _items = newItems;
                }
                else
                    _items = Array.Empty<T>();
        }
    }

    public SortedList(Comparison<T> comparison)
    {
        _items = Array.Empty<T>();
        _size = 0;
        _comparison = comparison;
    }

    public int Count => _size;
    public bool IsReadOnly => false;
    public void Add(T item)
    {
        if (_size == _items.Length) Grow(_size + 1);
        if (_size == 0)
        {
            _items[0] = item;
            _size++;
        }
        else
        {
            for (var i = 0; i < _size; i++)
                if (_comparison(item, _items[i]) < 0)
                {
                    Array.Copy(_items, i, _items, i + 1, _size - i);
                    _items[i] = item;
                    _size++;
                    return;
                }
            _items[_size++] = item;
        }
    }

    private void Grow(int capacity)
    {
        var newcapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

        if ((uint)newcapacity > Array.MaxLength) newcapacity = Array.MaxLength;

        if (newcapacity < capacity) newcapacity = capacity;

        Capacity = newcapacity;
    }

    public void Clear()
    {
        _items = Array.Empty<T>();
        _size = 0;
    }
    public bool Contains(T item) => IndexOf(item) != -1;
    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }
    public bool Remove(T item)
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
    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)_items[.._size].GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _items[.._size].GetEnumerator();

    public int IndexOf(T item) => Array.IndexOf(_items, item);

    public ReadOnlyCollection<T> AsReadOnly() => new(_items[.._size]);
}