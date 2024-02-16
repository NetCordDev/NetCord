namespace NetCord.Services.EnumTypeReaders;

internal unsafe class EnumTypeReaderManager<TTypeReader, TKey, TParameter, TConfiguration>(delegate*<TParameter, TKey> getKey, Func<TKey, TParameter, TConfiguration, TTypeReader> factory) where TTypeReader : IEnumTypeReader where TKey : notnull
{
    private readonly Dictionary<TKey, TTypeReader> _typeReaders = [];

    public TTypeReader GetTypeReader(TParameter parameter, TConfiguration configuration)
    {
        var typeReaders = _typeReaders;
        var key = getKey(parameter);

        TTypeReader? enumInfo;
        lock (typeReaders)
        {
            if (!typeReaders.TryGetValue(key, out enumInfo))
            {
                enumInfo = factory(key, parameter, configuration);
                typeReaders.Add(key, enumInfo);
            }
        }

        return enumInfo;
    }
}
