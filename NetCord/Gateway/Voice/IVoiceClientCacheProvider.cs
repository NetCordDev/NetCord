namespace NetCord.Gateway.Voice;

public interface IVoiceClientCacheProvider
{
    public IVoiceClientCache Create();
}
