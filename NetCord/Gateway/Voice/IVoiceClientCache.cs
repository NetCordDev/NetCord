namespace NetCord.Gateway.Voice;

public interface IVoiceClientCache : IDisposable
{
    public uint Ssrc { get; }
    public IReadOnlyDictionary<ulong, uint> Ssrcs { get; }
    public IReadOnlyDictionary<uint, ulong> Users { get; }

    public IVoiceClientCache CacheSelfSsrc(uint ssrc);
    public IVoiceClientCache CacheUser(uint ssrc, ulong userId);

    public IVoiceClientCache RemoveUser(uint ssrc, ulong userId);
}
