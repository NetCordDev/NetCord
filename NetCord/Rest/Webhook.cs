using NetCord.JsonModels;

namespace NetCord;

public class Webhook : Entity, IJsonModel<JsonWebhook>
{
    JsonWebhook IJsonModel<JsonWebhook>.JsonModel => _jsonModel;
    private readonly JsonWebhook _jsonModel;

    public Webhook(JsonWebhook jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (_jsonModel.Creator != null)
            Creator = new(_jsonModel.Creator, client);
    }

    public override Snowflake Id => _jsonModel.Id;

    public WebhookType Type => _jsonModel.Type;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public Snowflake? ChannelId => _jsonModel.ChannelId;

    public User? Creator { get; }

    public string? Name => _jsonModel.Name;

    public string? AvatarHash => _jsonModel.AvatarHash;

    public string? Token => _jsonModel.Token;

    public Snowflake? ApplicationId => _jsonModel.ApplicationId;

    public RestGuild? Guild => _jsonModel.Guild;

    public string? Url => _jsonModel.Url;

}