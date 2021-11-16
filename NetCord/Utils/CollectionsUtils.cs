namespace NetCord
{
    internal static class CollectionsUtils
    {
        internal static Dictionary<TKey, TSource> ToDictionaryOrEmpty<TSource, TKey>(this TSource[] source, Func<TSource, TKey> keySelector) where TKey : notnull
        {
            if (source == null)
                return new();
            else
                return source.ToDictionary(keySelector);
        }

        internal static Dictionary<TKey, TElement> ToDictionaryOrEmpty<TSource, TKey, TElement>(this TSource[] source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
        {
            if (source == null)
                return new();
            else
                return source.ToDictionary(keySelector, elementSelector);
        }

        public static IEnumerable<TResult> SelectOrEmpty<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
                return Enumerable.Empty<TResult>();
            else
                return source.Select(selector);
        }
    }
}
