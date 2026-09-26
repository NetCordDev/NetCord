namespace NetCord.Gateway.Voice;

public interface IVoiceClientCache : IDictionaryProvider, IDisposable
{
    public uint Ssrc { get; }
    public IReadOnlySet<ulong> Users { get; }
    public IReadOnlyDictionary<ulong, uint> UserSsrcs { get; }
    public IReadOnlyDictionary<uint, ulong> SsrcUsers { get; }

    public IVoiceClientCache CacheCurrentSsrc(uint ssrc);
    public IVoiceClientCache CacheUsers(IReadOnlyList<ulong> userIds);
    public IVoiceClientCache CacheUserSsrc(ulong userId, uint ssrc);

    public IVoiceClientCache RemoveUser(ulong userId);
}
