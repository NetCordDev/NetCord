using System.Collections;
using System.Collections.Concurrent;

using NetCord.Gateway.Voice.JsonModels;

namespace NetCord.Gateway.Voice;

internal sealed class EmptyConcurrentVoiceClientCacheProvider : ConcurrentVoiceClientCacheProvider
{
    public static EmptyConcurrentVoiceClientCacheProvider Instance { get; } = new();

    private EmptyConcurrentVoiceClientCacheProvider()
    {
    }

    public override ConcurrentVoiceClientCache Create() => new();
}

internal sealed class JsonConcurrentVoiceClientCacheProvider(JsonVoiceClientCache jsonModel) : ConcurrentVoiceClientCacheProvider
{
    public override ConcurrentVoiceClientCache Create() => new(jsonModel);
}

public abstract class ConcurrentVoiceClientCacheProvider : IVoiceClientCacheProvider
{
    public static ConcurrentVoiceClientCacheProvider Empty => EmptyConcurrentVoiceClientCacheProvider.Instance;

    public static ConcurrentVoiceClientCacheProvider FromJson(JsonVoiceClientCache jsonModel) => new JsonConcurrentVoiceClientCacheProvider(jsonModel);

    private protected ConcurrentVoiceClientCacheProvider()
    {
    }

    IVoiceClientCache IVoiceClientCacheProvider.Create() => Create();

    public abstract ConcurrentVoiceClientCache Create();
}

public sealed class ConcurrentVoiceClientCache : IVoiceClientCache
{
    internal ConcurrentVoiceClientCache()
    {
        _users = [];
        _userSsrcs = [];
        _ssrcUsers = [];
    }

    internal ConcurrentVoiceClientCache(JsonVoiceClientCache jsonModel)
    {
        _ssrc = jsonModel.Ssrc;
        _users = new(jsonModel.Users);
        _userSsrcs = new(jsonModel.UserSsrcs);
        _ssrcUsers = new(jsonModel.SsrcUsers);
    }

    public uint Ssrc => _ssrc;
    public IReadOnlySet<ulong> Users => _users;
    public IReadOnlyDictionary<ulong, uint> UserSsrcs => _userSsrcs;
    public IReadOnlyDictionary<uint, ulong> SsrcUsers => _ssrcUsers;

#pragma warning disable IDE0032 // Use auto property
    private uint _ssrc;
#pragma warning restore IDE0032 // Use auto property
    private readonly ConcurrentHashSet<ulong> _users;
    private readonly ConcurrentDictionary<ulong, uint> _userSsrcs;
    private readonly ConcurrentDictionary<uint, ulong> _ssrcUsers;

    public JsonVoiceClientCache ToJsonModel()
    {
        return new()
        {
            Ssrc = _ssrc,
            Users = _users.ToArray(),
            UserSsrcs = _userSsrcs.ToArray().ToDictionary(),
            SsrcUsers = _ssrcUsers.ToArray().ToDictionary(),
        };
    }

    public IVoiceClientCache CacheCurrentSsrc(uint ssrc)
    {
        _ssrc = ssrc;

        return this;
    }

    public IVoiceClientCache CacheUsers(IReadOnlyList<ulong> userIds)
    {
        int count = userIds.Count;
        for (int i = 0; i < count; i++)
            _users.Add(userIds[i]);

        return this;
    }

    public IVoiceClientCache CacheUserSsrc(ulong userId, uint ssrc)
    {
        _userSsrcs[userId] = ssrc;
        _ssrcUsers[ssrc] = userId;

        return this;
    }

    public IVoiceClientCache RemoveUser(ulong userId)
    {
        _users.Remove(userId);
        if (_userSsrcs.TryRemove(userId, out var ssrc))
            _ssrcUsers.TryRemove(ssrc, out _);

        return this;
    }

    public IReadOnlyDictionary<TKey, TValue> CreateDictionary<TSource, TKey, TValue>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        where TKey : notnull
        where TValue : class
    {
        return new ConcurrentDictionary<TKey, TValue>(source.Select(s => new KeyValuePair<TKey, TValue>(keySelector(s), elementSelector(s))));
    }

    public void Dispose()
    {
    }

    private class ConcurrentHashSet<T> : IReadOnlySet<T> where T : notnull
    {
        private readonly ConcurrentDictionary<T, byte> _storage;

        public ConcurrentHashSet()
        {
            _storage = [];
        }

        public ConcurrentHashSet(IEnumerable<T> collection)
        {
            _storage = new(collection.Select(item => new KeyValuePair<T, byte>(item, 0)));
        }

        public T[] ToArray() => [.. _storage.Keys];

        private static IReadOnlySet<T> GetSet(IEnumerable<T> collection) => collection as IReadOnlySet<T> ?? new HashSet<T>(collection);

        public int Count => _storage.Count;

        public bool Add(T item)
        {
            return _storage.TryAdd(item, 0);
        }

        public bool Remove(T item)
        {
            return _storage.TryRemove(item, out _);
        }

        public bool Contains(T item)
        {
            return _storage.ContainsKey(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _storage.Select(p => p.Key).GetEnumerator();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return GetSet(other).IsProperSupersetOf(_storage.Keys);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return GetSet(other).IsProperSubsetOf(_storage.Keys);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return GetSet(other).IsSupersetOf(_storage.Keys);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return other.All(Contains);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return other.Any(Contains);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return GetSet(other).SetEquals(_storage.Keys);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
