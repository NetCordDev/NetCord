namespace NetCord;

public class GuildWelcomeScreenChannel(JsonModels.JsonWelcomeScreenChannel jsonModel) : Entity, IJsonModel<JsonModels.JsonWelcomeScreenChannel>
{
    JsonModels.JsonWelcomeScreenChannel IJsonModel<JsonModels.JsonWelcomeScreenChannel>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.ChannelId;

    public string Description => jsonModel.Description;

    public ulong? EmojiId => jsonModel.EmojiId;

    public string? EmojiName => jsonModel.EmojiName;
}
