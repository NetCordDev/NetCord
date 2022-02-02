using System.Collections.Immutable;

namespace NetCord;

internal static class CollectionsUtils
{
    public static Dictionary<TKey, TSource> ToDictionaryOrEmpty<TSource, TKey>(this IEnumerable<TSource>? source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        if (source == null)
            return new();
        else
            return source.ToDictionary(keySelector);
    }

    public static Dictionary<TKey, TElement> ToDictionaryOrEmpty<TSource, TKey, TElement>(this IEnumerable<TSource>? source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        if (source == null)
            return new();
        else
            return source.ToDictionary(keySelector, elementSelector);
    }

    public static IEnumerable<TResult> SelectOrEmpty<TSource, TResult>(this IEnumerable<TSource>? source, Func<TSource, TResult> selector)
    {
        if (source == null)
            return Enumerable.Empty<TResult>();
        else
            return source.Select(selector);
    }

    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionaryOrEmpty<TSource, TKey>(this IEnumerable<TSource>? source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        if (source == null)
            return ImmutableDictionary<TKey, TSource>.Empty.WithComparers(null, new ReferenceEqualityComparer<TSource>());
        else
            return source.ToImmutableDictionary(keySelector).WithComparers(null, new ReferenceEqualityComparer<TSource>());
    }

    public static ImmutableDictionary<TKey, TElement> ToImmutableDictionaryOrEmpty<TSource, TKey, TElement>(this IEnumerable<TSource>? source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        if (source == null)
            return ImmutableDictionary<TKey, TElement>.Empty.WithComparers(null, new ReferenceEqualityComparer<TElement>());
        else
            return source.ToImmutableDictionary(keySelector, elementSelector, null, new ReferenceEqualityComparer<TElement>());
    }

    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
    {
        return source.ToImmutableDictionary(keySelector).WithComparers(null, new ReferenceEqualityComparer<TSource>());
    }

    public static ImmutableDictionary<TKey, TElement> ToImmutableDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        return source.ToImmutableDictionary(keySelector, elementSelector, null, new ReferenceEqualityComparer<TElement>());
    }

    public static ImmutableDictionary<TKey, TElement> CreateImmutableDictionary<TKey, TElement>() where TKey : notnull => ImmutableDictionary<TKey, TElement>.Empty.WithComparers(null, new ReferenceEqualityComparer<TElement>());
}