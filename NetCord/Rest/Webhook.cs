using NetCord.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a webhook, a low-effort way to post messages to channels in Discord. They do not require a bot user or authentication to use.
/// </summary>
public partial class Webhook : ClientEntity, IJsonModel<JsonWebhook>
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

    /// <summary>
    /// The ID of the webhook.
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The type of the webhook.
    /// </summary>
    public WebhookType Type => _jsonModel.Type;

    /// <summary>
    /// The guild ID this webhook targets. Can be <see langword="null"/>.
    /// </summary>
    public ulong? GuildId => _jsonModel.GuildId;

    /// <summary>
    /// The channel ID this webhook targets. Can be <see langword="null"/>.
    /// </summary>
    public ulong? ChannelId => _jsonModel.ChannelId;

    /// <summary>
    /// The <see cref="User"/> this webhook was created by.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> if the parent object was retrieved using its token.
    /// </remarks>
    public User? Creator { get; }

    /// <summary>
    /// The name of the webhook.
    /// </summary>
    /// <remarks>
    /// A webhook name is valid if:
    /// <list type="bullet">
    ///     <item>
    ///         The string length is between 1 and 80 characters.
    ///     </item>
    ///     <item>
    ///         It does not contain the substrings <c>clyde</c> or <c>discord</c>.
    ///     </item>
    ///     <item>
    ///         It follows Discord's nickname guidelines, found <see href="https://docs.discord.com/developers/resources/user#usernames-and-nicknames">here</see>.
    ///     </item>
    /// </list>
    /// </remarks>
    public string? Name => _jsonModel.Name;

    /// <summary>
    /// The default user avatar hash of the webhook.
    /// </summary>
    public string? AvatarHash => _jsonModel.AvatarHash;

    /// <summary>
    /// The ID of the bot or OAuth2 application that created this webhook.
    /// </summary>
    public ulong? ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// The guild of the channel followed by this webhook.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> if <see cref="Type"/> is not <see cref="WebhookType.ChannelFollower"/>,
    /// or if the <see cref="Creator"/> has lost access to the guild where the <see cref="Channel"/> resides.
    /// </remarks>
    public RestGuild? Guild { get; }

    /// <summary>
    /// The channel that this webhook is following.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> if <see cref="Type"/> is not <see cref="WebhookType.ChannelFollower"/>,
    /// or if the <see cref="Creator"/> has lost access to the guild where the <see cref="Channel"/> resides.
    /// </remarks>
    public Channel? Channel { get; }

    /// <summary>
    /// The URL used for executing the webhook.
    /// </summary>
    public string? Url => _jsonModel.Url;

    public static Webhook CreateFromJson(JsonWebhook jsonModel, RestClient client) => jsonModel.Type switch
    {
        WebhookType.Incoming => new IncomingWebhook(jsonModel, client),
        _ => new Webhook(jsonModel, client),
    };
}
