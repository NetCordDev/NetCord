namespace NetCord;

public class GuildWelcomeScreenChannel : Entity, IJsonModel<JsonModels.JsonWelcomeScreenChannel>
{
    JsonModels.JsonWelcomeScreenChannel IJsonModel<JsonModels.JsonWelcomeScreenChannel>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonWelcomeScreenChannel _jsonModel;

    public override Snowflake Id => _jsonModel.ChannelId;

    public string Description => _jsonModel.Description;

    public Snowflake? EmojiId => _jsonModel.EmojiId;

    public string? EmojiName => _jsonModel.EmojiName;

    public GuildWelcomeScreenChannel(JsonModels.JsonWelcomeScreenChannel jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
