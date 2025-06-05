using System.Collections.Immutable;

using NetCord.Gateway.Voice.JsonModels;

namespace NetCord.Gateway.Voice;

public sealed record VoiceClientCache : IVoiceClientCache
{
    public uint Ssrc => _ssrc;
    public IReadOnlyDictionary<ulong, uint> Ssrcs => _ssrcs;
    public IReadOnlyDictionary<uint, ulong> Users => _users;

#pragma warning disable IDE0032 // Use auto property
    private uint _ssrc;
#pragma warning restore IDE0032 // Use auto property
    private ImmutableDictionary<ulong, uint> _ssrcs;
    private ImmutableDictionary<uint, ulong> _users;

    public VoiceClientCache()
    {
        _ssrcs = ImmutableDictionary<ulong, uint>.Empty;
        _users = ImmutableDictionary<uint, ulong>.Empty;
    }

    public VoiceClientCache(JsonVoiceClientCache jsonModel)
    {
        _ssrc = jsonModel.Ssrc;
        _ssrcs = jsonModel.Ssrcs;
        _users = jsonModel.Users;
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
        return this with
        {
            _ssrc = ssrc,
        };
    }

    public IVoiceClientCache CacheUser(ulong userId, uint ssrc)
    {
        return this with
        {
            _ssrcs = _ssrcs.SetItem(userId, ssrc),
            _users = _users.SetItem(ssrc, userId),
        };
    }

    public IVoiceClientCache RemoveUser(ulong userId)
    {
        var ssrcs = _ssrcs;

        if (!ssrcs.TryGetValue(userId, out var ssrc))
            return this;

        return this with
        {
            _ssrcs = ssrcs.Remove(userId),
            _users = _users.Remove(ssrc),
        };
    }

    public void Dispose()
    {
    }
}
