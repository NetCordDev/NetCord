namespace NetCord.Services.EnumTypeReaders;

internal unsafe class EnumTypeReaderManager<TTypeReader, TKey, TParameter, TConfiguration> where TTypeReader : IEnumTypeReader where TKey : notnull
{
    private readonly delegate*<TParameter, TKey> _getKey;
    private readonly Func<TKey, TParameter, TConfiguration, TTypeReader> _factory;
    private readonly Dictionary<TKey, TTypeReader> _typeReaders;

    public EnumTypeReaderManager(delegate*<TParameter, TKey> getKey, Func<TKey, TParameter, TConfiguration, TTypeReader> factory)
    {
        _getKey = getKey;
        _factory = factory;
        _typeReaders = new();
    }

    public TTypeReader GetTypeReader(TParameter parameter, TConfiguration configuration)
    {
        var typeReaders = _typeReaders;
        var key = _getKey(parameter);

        TTypeReader? enumInfo;
        lock (typeReaders)
        {
            if (!typeReaders.TryGetValue(key, out enumInfo))
            {
                enumInfo = _factory(key, parameter, configuration);
                typeReaders.Add(key, enumInfo);
            }
        }

        return enumInfo;
    }
}
