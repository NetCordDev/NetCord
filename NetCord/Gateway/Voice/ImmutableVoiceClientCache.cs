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
        _users = ImmutableHashSet<ulong>.Empty;
        _userSsrcs = ImmutableDictionary<ulong, uint>.Empty;
        _ssrcUsers = ImmutableDictionary<uint, ulong>.Empty;
    }

    private ImmutableVoiceClientCache(JsonVoiceClientCache jsonModel)
    {
        _ssrc = jsonModel.Ssrc;
        _users = [.. jsonModel.Users];
        _userSsrcs = jsonModel.UserSsrcs.ToImmutableDictionary();
        _ssrcUsers = jsonModel.SsrcUsers.ToImmutableDictionary();
    }

    private ImmutableVoiceClientCache(uint ssrc, ImmutableHashSet<ulong> users, ImmutableDictionary<ulong, uint> userSsrcs, ImmutableDictionary<uint, ulong> ssrcUsers)
    {
        _ssrc = ssrc;
        _users = users;
        _userSsrcs = userSsrcs;
        _ssrcUsers = ssrcUsers;
    }

    private static ImmutableVoiceClientCache Create(uint ssrc, ImmutableHashSet<ulong> users, ImmutableDictionary<ulong, uint> userSsrcs, ImmutableDictionary<uint, ulong> ssrcUsers)
    {
        return new(ssrc, users, userSsrcs, ssrcUsers);
    }

    public uint Ssrc => _ssrc;
    public IReadOnlySet<ulong> Users => _users;
    public IReadOnlyDictionary<ulong, uint> UserSsrcs => _userSsrcs;
    public IReadOnlyDictionary<uint, ulong> SsrcUsers => _ssrcUsers;

#pragma warning disable IDE0032 // Use auto property
    private readonly uint _ssrc;
#pragma warning restore IDE0032 // Use auto property
    private readonly ImmutableHashSet<ulong> _users;
    private readonly ImmutableDictionary<ulong, uint> _userSsrcs;
    private readonly ImmutableDictionary<uint, ulong> _ssrcUsers;

    public JsonVoiceClientCache ToJsonModel()
    {
        return new()
        {
            Ssrc = _ssrc,
            Users = _users.ToArray(),
            UserSsrcs = _userSsrcs,
            SsrcUsers = _ssrcUsers,
        };
    }

    public IVoiceClientCache CacheCurrentSsrc(uint ssrc)
    {
        return Create(ssrc,
                      _users,
                      _userSsrcs,
                      _ssrcUsers);
    }

    public IVoiceClientCache CacheUsers(IReadOnlyList<ulong> userIds)
    {
        return Create(_ssrc,
                      _users.Union(userIds),
                      _userSsrcs,
                      _ssrcUsers);
    }

    public IVoiceClientCache CacheUserSsrc(ulong userId, uint ssrc)
    {
        return Create(_ssrc,
                      _users,
                      _userSsrcs.SetItem(userId, ssrc),
                      _ssrcUsers.SetItem(ssrc, userId));
    }

    public IVoiceClientCache RemoveUser(ulong userId)
    {
        var userSsrcs = _userSsrcs;

        if (!userSsrcs.TryGetValue(userId, out var ssrc))
            return Create(_ssrc,
                          _users.Remove(userId),
                          userSsrcs,
                          _ssrcUsers);

        return Create(_ssrc,
                      _users.Remove(userId),
                      userSsrcs.Remove(userId),
                      _ssrcUsers.Remove(ssrc));
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
