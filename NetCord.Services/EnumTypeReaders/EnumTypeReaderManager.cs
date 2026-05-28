using System.Collections.Concurrent;

namespace NetCord.Services.EnumTypeReaders;

internal unsafe class EnumTypeReaderManager<TTypeReader, TKey, TParameter, TConfiguration>(delegate*<TParameter, TKey> getKey, Func<TKey, TParameter, TConfiguration, TTypeReader> factory)
    where TTypeReader : IEnumTypeReader where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TTypeReader> _typeReaders = [];

    public TTypeReader GetTypeReader(TParameter parameter, TConfiguration configuration)
    {
        return _typeReaders.GetOrAdd(getKey(parameter),
                                     static (key, arg) => arg.Factory(key, arg.Parameter, arg.Configuration),
                                     (Factory: factory, Parameter: parameter, Configuration: configuration));
    }
}
