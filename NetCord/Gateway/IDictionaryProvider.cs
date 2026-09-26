namespace NetCord.Gateway;

public interface IDictionaryProvider
{
    public static IDictionaryProvider OfDictionary { get; } = new DictionaryProvider();

    public IReadOnlyDictionary<TKey, TElement> CreateDictionary<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        where TKey : notnull
        where TElement : class;
}

internal class DictionaryProvider : IDictionaryProvider
{
    public IReadOnlyDictionary<TKey, TElement> CreateDictionary<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        where TKey : notnull
        where TElement : class
    {
        return source.ToDictionary(keySelector, elementSelector);
    }
}
