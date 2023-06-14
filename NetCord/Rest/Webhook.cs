using NetCord.JsonModels;

namespace NetCord.Rest;

public class Webhook : ClientEntity, IJsonModel<JsonWebhook>
{
    JsonWebhook IJsonModel<JsonWebhook>.JsonModel => _jsonModel;
    private protected readonly JsonWebhook _jsonModel;

    public Webhook(JsonWebhook jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var creator = jsonModel.Creator;
        if (creator is not null)
            Creator = new(creator, client);

        var guild = jsonModel.Guild;
        if (guild is not null)
            Guild = new(guild, client);

        var channel = jsonModel.Channel;
        if (channel is not null)
            Channel = Channel.CreateFromJson(channel, client);
    }

    public override ulong Id => _jsonModel.Id;

    public WebhookType Type => _jsonModel.Type;

    public ulong? GuildId => _jsonModel.GuildId;

    public ulong? ChannelId => _jsonModel.ChannelId;

    public User? Creator { get; }

    public string? Name => _jsonModel.Name;

    public string? AvatarHash => _jsonModel.AvatarHash;

    public ulong? ApplicationId => _jsonModel.ApplicationId;

    public RestGuild? Guild { get; }

    public Channel? Channel { get; }

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
