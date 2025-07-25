using System.Collections.Immutable;

using NetCord.Gateway.Voice.JsonModels;

namespace NetCord.Gateway.Voice;

public sealed class ImmutableVoiceClientCache : IVoiceClientCache
{
    public uint Ssrc => _ssrc;
    public IReadOnlyDictionary<ulong, uint> Ssrcs => _ssrcs;
    public IReadOnlyDictionary<uint, ulong> Users => _users;

#pragma warning disable IDE0032 // Use auto property
    private readonly uint _ssrc;
#pragma warning restore IDE0032 // Use auto property
    private readonly ImmutableDictionary<ulong, uint> _ssrcs;
    private readonly ImmutableDictionary<uint, ulong> _users;

    public ImmutableVoiceClientCache()
    {
        _ssrcs = ImmutableDictionary<ulong, uint>.Empty;
        _users = ImmutableDictionary<uint, ulong>.Empty;
    }

    public ImmutableVoiceClientCache(JsonVoiceClientCache jsonModel)
    {
        _ssrc = jsonModel.Ssrc;
        _ssrcs = jsonModel.Ssrcs.ToImmutableDictionary();
        _users = jsonModel.Users.ToImmutableDictionary();
    }

    private ImmutableVoiceClientCache(uint ssrc, ImmutableDictionary<ulong, uint> ssrcs, ImmutableDictionary<uint, ulong> users)
    {
        _ssrc = ssrc;
        _ssrcs = ssrcs;
        _users = users;
    }

    private static ImmutableVoiceClientCache Create(uint ssrc, ImmutableDictionary<ulong, uint> ssrcs, ImmutableDictionary<uint, ulong> users)
    {
        return new(ssrc, ssrcs, users);
    }

    public JsonVoiceClientCache ToJsonModel()
    {
        return new()
        {
            Ssrc = _ssrc,
            Ssrcs = _ssrcs,
            Users = _users,
        };
    }

    public IVoiceClientCache CacheCurrentSsrc(uint ssrc)
    {
        return Create(ssrc,
                      _ssrcs,
                      _users);
    }

    public IVoiceClientCache CacheUser(ulong userId, uint ssrc)
    {
        return Create(_ssrc,
                      _ssrcs.SetItem(userId, ssrc),
                      _users.SetItem(ssrc, userId));
    }

    public IVoiceClientCache RemoveUser(ulong userId)
    {
        var ssrcs = _ssrcs;

        if (!ssrcs.TryGetValue(userId, out var ssrc))
            return this;

        return Create(_ssrc,
                      ssrcs.Remove(userId),
                      _users.Remove(ssrc));
    }

    public IReadOnlyDictionary<TKey, TValue> CreateDictionary<TSource, TKey, TValue>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector) where TKey : notnull where TValue : class
    {
        return source.ToImmutableDictionary(keySelector, elementSelector);
    }

    public void Dispose()
    {
    }
}
