namespace NetCord;

public class GuildWidget
{
    private readonly JsonModels.JsonGuildWidget _jsonEntity;

    public bool Enabled => _jsonEntity.Enabled;

    public DiscordId? ChannelId => _jsonEntity.ChannelId;

    internal GuildWidget(JsonModels.JsonGuildWidget jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}