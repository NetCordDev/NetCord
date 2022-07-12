using NetCord.JsonModels;

namespace NetCord.Rest;

public class Webhook : ClientEntity, IJsonModel<JsonWebhook>
{
    JsonWebhook IJsonModel<JsonWebhook>.JsonModel => _jsonModel;
    private protected readonly JsonWebhook _jsonModel;

    public Webhook(JsonWebhook jsonModel, RestClient client) : base(client)
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

    

    public Snowflake? ApplicationId => _jsonModel.ApplicationId;

    public RestGuild? Guild => _jsonModel.Guild;

    public string? Url => _jsonModel.Url;

    public static Webhook CreateFromJson(JsonWebhook jsonModel, RestClient client) => jsonModel.Type switch
    {
        WebhookType.Incoming => new IncomingWebhook(jsonModel, client),
        _ => new(jsonModel, client),
    };

    #region Webhook
    public Task<Webhook> ModifyAsync(Action<WebhookOptions> action, RequestProperties? properties = null) => _client.ModifyWebhookAsync(Id, action, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteWebhookAsync(Id, properties);
    #endregion
}