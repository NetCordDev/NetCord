using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace NetCord;

internal static class CollectionsUtils
{
    #region IEnumerable<out T>
    public static Dictionary<TKey, TSource> ToDictionaryOrEmpty<TSource, TKey>(this IEnumerable<TSource>? source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        if (source is null)
            return [];
        else
            return source.ToDictionary(keySelector);
    }

    public static Dictionary<TKey, TElement> ToDictionaryOrEmpty<TSource, TKey, TElement>(this IEnumerable<TSource>? source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        if (source is null)
            return [];
        else
            return source.ToDictionary(keySelector, elementSelector);
    }

    public static IEnumerable<TResult> SelectOrEmpty<TSource, TResult>(this IEnumerable<TSource>? source, Func<TSource, TResult> selector)
    {
        if (source is null)
            return [];
        else
            return source.Select(selector);
    }

    public static ImmutableDictionary<TKey, TElement> ToImmutableDictionaryOrEmpty<TSource, TKey, TElement>(this IEnumerable<KeyValuePair<TKey, TSource>>? source, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        if (source is null)
            return CreateImmutableDictionary<TKey, TElement>();
        else
            return CreateImmutableDictionary<TKey, TElement>().AddRange(source.Select(p => new KeyValuePair<TKey, TElement>(p.Key, elementSelector(p.Value))));
    }

    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionaryOrEmpty<TSource, TKey>(this IEnumerable<TSource>? source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        if (source is null)
            return CreateImmutableDictionary<TKey, TSource>();
        else
            return source.ToImmutableDictionary(keySelector);
    }

    public static ImmutableDictionary<TKey, TElement> ToImmutableDictionaryOrEmpty<TSource, TKey, TElement>(this IEnumerable<TSource>? source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        if (source is null)
            return CreateImmutableDictionary<TKey, TElement>();
        else
            return source.ToImmutableDictionary(keySelector, elementSelector);
    }

    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        return CreateImmutableDictionary<TKey, TSource>().AddRange(source.Select(s => new KeyValuePair<TKey, TSource>(keySelector(s), s)));
    }

    public static ImmutableDictionary<TKey, TElement> ToImmutableDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        return CreateImmutableDictionary<TKey, TElement>().AddRange(source.Select(s => new KeyValuePair<TKey, TElement>(keySelector(s), elementSelector(s))));
    }

    public static ImmutableDictionary<TKey, TElement> ToImmutableDictionary<TSource, TKey, TElement>(this IEnumerable<KeyValuePair<TKey, TSource>> source, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        return CreateImmutableDictionary<TKey, TElement>().AddRange(source.Select(p => new KeyValuePair<TKey, TElement>(p.Key, elementSelector(p.Value))));
    }
    #endregion

    #region ImmutableArray<T>
    public static Dictionary<TKey, TSource> ToDictionaryOrEmpty<TSource, TKey>(this ImmutableArray<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        if (source.IsDefaultOrEmpty)
            return [];
        else
            return source.ToDictionary(keySelector);
    }

    public static Dictionary<TKey, TElement> ToDictionaryOrEmpty<TSource, TKey, TElement>(this ImmutableArray<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        if (source.IsDefaultOrEmpty)
            return [];
        else
            return source.ToDictionary(keySelector, elementSelector);
    }

    public static IEnumerable<TResult> SelectOrEmpty<TSource, TResult>(this ImmutableArray<TSource> source, Func<TSource, TResult> selector)
    {
        if (source.IsDefaultOrEmpty)
            return [];
        else
            return source.Select(selector);
    }

    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionaryOrEmpty<TSource, TKey>(this ImmutableArray<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        if (source.IsDefaultOrEmpty)
            return CreateImmutableDictionary<TKey, TSource>();
        else
            return source.ToImmutableDictionary(keySelector);
    }

    public static ImmutableDictionary<TKey, TElement> ToImmutableDictionaryOrEmpty<TSource, TKey, TElement>(this ImmutableArray<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        if (source.IsDefaultOrEmpty)
            return CreateImmutableDictionary<TKey, TElement>();
        else
            return source.ToImmutableDictionary(keySelector, elementSelector);
    }

    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionary<TSource, TKey>(this ImmutableArray<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        return CreateImmutableDictionary<TKey, TSource>().AddRange(source.Select(s => new KeyValuePair<TKey, TSource>(keySelector(s), s)));
    }

    public static ImmutableDictionary<TKey, TElement> ToImmutableDictionary<TSource, TKey, TElement>(this ImmutableArray<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        return CreateImmutableDictionary<TKey, TElement>().AddRange(source.Select(s => new KeyValuePair<TKey, TElement>(keySelector(s), elementSelector(s))));
    }
    #endregion

    public static ImmutableDictionary<TKey, TElement> CreateImmutableDictionary<TKey, TElement>() where TKey : notnull => ImmutableDictionary<TKey, TElement>.Empty.WithComparers(null, new ReferenceEqualityComparer<TElement>());

    private class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
        public int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
    }

    public static IEnumerable<T> GetReversedIEnumerable<T>(this T[] source)
    {
        for (int i = source.Length - 1; i >= 0; i--)
            yield return source[i];
    }
}
