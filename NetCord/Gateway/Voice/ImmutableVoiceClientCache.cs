using System.Collections.Immutable;

using NetCord.Gateway.Voice.JsonModels;

namespace NetCord.Gateway.Voice;

internal sealed class EmptyImmutableVoiceClientCacheProvider : ImmutableVoiceClientCacheProvider
{
    public static EmptyImmutableVoiceClientCacheProvider Instance { get; } = new();

    private EmptyImmutableVoiceClientCacheProvider()
    {
    }

    public override ImmutableVoiceClientCache Create() => ImmutableVoiceClientCache.Empty;
}

internal sealed class JsonImmutableVoiceClientCacheProvider(JsonVoiceClientCache jsonModel) : ImmutableVoiceClientCacheProvider
{
    public override ImmutableVoiceClientCache Create() => ImmutableVoiceClientCache.FromJson(jsonModel);
}

public abstract class ImmutableVoiceClientCacheProvider : IVoiceClientCacheProvider
{
    public static ImmutableVoiceClientCacheProvider Empty => EmptyImmutableVoiceClientCacheProvider.Instance;

    public static ImmutableVoiceClientCacheProvider FromJson(JsonVoiceClientCache jsonModel) => new JsonImmutableVoiceClientCacheProvider(jsonModel);

    private protected ImmutableVoiceClientCacheProvider()
    {
    }

    IVoiceClientCache IVoiceClientCacheProvider.Create() => Create();

    public abstract ImmutableVoiceClientCache Create();
}

public sealed class ImmutableVoiceClientCache : IVoiceClientCache
{
    internal static ImmutableVoiceClientCache Empty { get; } = new();

    internal static ImmutableVoiceClientCache FromJson(JsonVoiceClientCache jsonModel)
    {
        return new(jsonModel);
    }

    private ImmutableVoiceClientCache()
    {
        _ssrcs = ImmutableDictionary<ulong, uint>.Empty;
        _users = ImmutableDictionary<uint, ulong>.Empty;
    }

    private ImmutableVoiceClientCache(JsonVoiceClientCache jsonModel)
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

    public uint Ssrc => _ssrc;
    public IReadOnlyDictionary<ulong, uint> Ssrcs => _ssrcs;
    public IReadOnlyDictionary<uint, ulong> Users => _users;

#pragma warning disable IDE0032 // Use auto property
    private readonly uint _ssrc;
#pragma warning restore IDE0032 // Use auto property
    private readonly ImmutableDictionary<ulong, uint> _ssrcs;
    private readonly ImmutableDictionary<uint, ulong> _users;

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

    public IReadOnlyDictionary<TKey, TValue> CreateDictionary<TSource, TKey, TValue>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        where TKey : notnull
        where TValue : class
    {
        return source.ToImmutableDictionary(keySelector, elementSelector);
    }

    public void Dispose()
    {
    }
}
