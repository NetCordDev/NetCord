using System.Collections;

namespace NetCord;

internal sealed class EntityArrayWrapper<T>(T[] array) : IReadOnlyList<ulong> where T : Entity
{
    public ulong this[int index] => array[index].Id;

    public int Count => array.Length;

    public IEnumerator<ulong> GetEnumerator()
    {
        int length = array.Length;
        for (int i = 0; i < length; i++)
            yield return array[i].Id;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
