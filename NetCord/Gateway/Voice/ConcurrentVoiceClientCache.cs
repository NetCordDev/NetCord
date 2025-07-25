using System.Collections.Concurrent;

using NetCord.Gateway.Voice.JsonModels;

namespace NetCord.Gateway.Voice;

public sealed class ConcurrentVoiceClientCache : IVoiceClientCache
{
    public uint Ssrc => _ssrc;
    public IReadOnlyDictionary<ulong, uint> Ssrcs => _ssrcs;
    public IReadOnlyDictionary<uint, ulong> Users => _users;

#pragma warning disable IDE0032 // Use auto property
    private uint _ssrc;
#pragma warning restore IDE0032 // Use auto property
    private readonly ConcurrentDictionary<ulong, uint> _ssrcs;
    private readonly ConcurrentDictionary<uint, ulong> _users;

    public ConcurrentVoiceClientCache()
    {
        _ssrcs = new();
        _users = new();
    }

    public ConcurrentVoiceClientCache(JsonVoiceClientCache jsonModel)
    {
        _ssrc = jsonModel.Ssrc;
        _ssrcs = new(jsonModel.Ssrcs);
        _users = new(jsonModel.Users);
    }

    public JsonVoiceClientCache ToJsonModel()
    {
        return new()
        {
            Ssrc = _ssrc,
            Ssrcs = _ssrcs.ToDictionary(),
            Users = _users.ToDictionary(),
        };
    }

    public IVoiceClientCache CacheCurrentSsrc(uint ssrc)
    {
        _ssrc = ssrc;

        return this;
    }

    public IVoiceClientCache CacheUser(ulong userId, uint ssrc)
    {
        _ssrcs[userId] = ssrc;
        _users[ssrc] = userId;

        return this;
    }

    public IVoiceClientCache RemoveUser(ulong userId)
    {
        if (_ssrcs.TryRemove(userId, out var ssrc))
            _users.TryRemove(ssrc, out _);

        return this;
    }

    public IReadOnlyDictionary<TKey, TValue> CreateDictionary<TSource, TKey, TValue>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector) where TKey : notnull where TValue : class
    {
        return new ConcurrentDictionary<TKey, TValue>(source.Select(s => new KeyValuePair<TKey, TValue>(keySelector(s), elementSelector(s))));
    }

    public void Dispose()
    {
    }
}
