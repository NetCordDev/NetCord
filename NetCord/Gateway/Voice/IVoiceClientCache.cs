namespace NetCord.Gateway.Voice;

public interface IVoiceClientCache : IDictionaryProvider, IDisposable
{
    public uint Ssrc { get; }
    public IReadOnlyDictionary<ulong, uint> Ssrcs { get; }
    public IReadOnlyDictionary<uint, ulong> Users { get; }

    public IVoiceClientCache CacheCurrentSsrc(uint ssrc);
    public IVoiceClientCache CacheUser(ulong userId, uint ssrc);

    public IVoiceClientCache RemoveUser(ulong userId);
}
