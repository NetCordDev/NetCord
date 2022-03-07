namespace NetCord;

public class VoiceServerUpdateEventArgs
{
    private readonly JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs _jsonEntity;

    internal VoiceServerUpdateEventArgs(JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public string Token => _jsonEntity.Token;

    public DiscordId GuildId => _jsonEntity.GuildId;

    public string? Endpoint => _jsonEntity.Endpoint;
}
