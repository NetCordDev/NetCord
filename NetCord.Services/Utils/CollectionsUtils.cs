using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Utils;

internal static class CollectionsUtils
{
    public static Dictionary<TKey, IReadOnlyList<TSource>> ToRankedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        Dictionary<TKey, IReadOnlyList<TSource>> result = [];
        foreach (var s in source)
        {
            var key = keySelector(s);
            if (result.TryGetValue(key, out var list))
                ((List<TSource>)list).Add(s);
            else
            {
                list = new List<TSource>(1)
                {
                    s
                };
                result.Add(key, list);
            }
        }
        return result;
    }

    public static Dictionary<TKey, IReadOnlyList<TElement>> ToRankedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        Dictionary<TKey, IReadOnlyList<TElement>> result = [];
        foreach (var s in source)
        {
            var key = keySelector(s);
            var element = elementSelector(s);
            if (result.TryGetValue(key, out var list))
                ((List<TElement>)list).Add(element);
            else
            {
                list = new List<TElement>(1)
                {
                    element
                };
                result.Add(key, list);
            }
        }
        return result;
    }

    public static IEnumerable<(TFirst, TSecond?)> ZipAndSecondNull<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
    {
        using var firstEnumerator = first.GetEnumerator();
        using var secondEnumerator = second.GetEnumerator();
        while (firstEnumerator.MoveNext())
        {
            secondEnumerator.MoveNext();
            yield return (firstEnumerator.Current, secondEnumerator.Current);
        }
    }

    public static bool TryGetSingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, [MaybeNullWhen(false)] out TSource value)
    {
        using (var e = source.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var result = e.Current;
                if (predicate(result))
                {
                    value = result;
                    while (e.MoveNext())
                    {
                        if (predicate(e.Current))
                        {
                            value = default;
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
}
