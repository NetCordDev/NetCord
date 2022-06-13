using NetCord.JsonModels;

namespace NetCord;

public class Webhook : IJsonModel<JsonWebhook>
{
    JsonWebhook IJsonModel<JsonWebhook>.JsonModel => _jsonModel;
    private readonly JsonWebhook _jsonModel;

    public Webhook(JsonWebhook jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public WebhookType Type => _jsonModel.Type;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public Snowflake? ChannelId => _jsonModel.ChannelId;

    public JsonUser? Creator => _jsonModel.Creator;

    public string? Name => _jsonModel.Name;

    public string? AvatarHash => _jsonModel.AvatarHash;

    public string? Token => _jsonModel.Token;

    public Snowflake? ApplicationId => _jsonModel.ApplicationId;

    public RestGuild? Guild => _jsonModel.Guild;

    public string? Url => _jsonModel.Url;
}