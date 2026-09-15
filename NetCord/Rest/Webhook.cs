using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a webhook, a low-effort way to post messages to channels in Discord. They do not require a bot user or authentication to use.
/// </summary>
public partial class Webhook(JsonWebhook jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonWebhook>
{
    JsonWebhook IJsonModel<JsonWebhook>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the webhook.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The type of the webhook.
    /// </summary>
    public WebhookType Type => jsonModel.Type;

    /// <summary>
    /// The guild ID this webhook targets. Can be <see langword="null"/>.
    /// </summary>
    public ulong? GuildId => jsonModel.GuildId;

    /// <summary>
    /// The channel ID this webhook targets. Can be <see langword="null"/>.
    /// </summary>
    public ulong? ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// The <see cref="User"/> this webhook was created by.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> if the webhook was retrieved using its token.
    /// </remarks>
    public User? Creator { get; } = jsonModel.Creator is { } creator ? new(creator, client) : null;

    /// <summary>
    /// The default name of the webhook.
    /// </summary>
    /// <remarks>
    /// A webhook name is valid if:
    /// <list type="bullet">
    ///     <item>
    ///         The string length is between 1 and 80 characters.
    ///     </item>
    ///     <item>
    ///         It does not contain the substrings <c>clyde</c> or <c>discord</c> (case-insensitive).
    ///     </item>
    ///     <item>
    ///         It follows Discord's nickname guidelines, found <see href="https://docs.discord.com/developers/resources/user#usernames-and-nicknames">here</see>.
    ///     </item>
    /// </list>
    /// </remarks>
    public string? Name => jsonModel.Name;

    /// <summary>
    /// The default user avatar hash of the webhook.
    /// </summary>
    public string? AvatarHash => jsonModel.AvatarHash;

    /// <summary>
    /// The ID of the bot or OAuth2 application that created this webhook.
    /// </summary>
    public ulong? ApplicationId => jsonModel.ApplicationId;

    /// <summary>
    /// The guild of the channel followed by this webhook.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> if <see cref="Type"/> is not <see cref="WebhookType.ChannelFollower"/>,
    /// or if the <see cref="Creator"/> has lost access to the guild where the <see cref="Channel"/> resides.
    /// </remarks>
    public RestGuild? Guild { get; } = jsonModel.Guild is { } guild ? new(guild, client) : null;

    /// <summary>
    /// The channel that this webhook is following.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> if <see cref="Type"/> is not <see cref="WebhookType.ChannelFollower"/>,
    /// or if the <see cref="Creator"/> has lost access to the guild where the <see cref="Channel"/> resides.
    /// </remarks>
    public Channel? Channel { get; } = jsonModel.Channel is { } channel ? Channel.CreateFromJson(channel, client) : null;

    /// <summary>
    /// The URL used for executing the webhook.
    /// </summary>
    public string? Url => jsonModel.Url;

    public static Webhook CreateFromJson(JsonWebhook jsonModel, RestClient client) => jsonModel.Type switch
    {
        WebhookType.Incoming => new IncomingWebhook(jsonModel, client),
        _ => new Webhook(jsonModel, client),
    };
}
