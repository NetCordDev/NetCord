using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents an incomplete <see cref="Message"/> object, with missing fields. Sent during <see cref="GatewayClient.MessageUpdate"/> events.
/// </summary>
public partial interface IPartialMessage : IEntity
{
    public static IPartialMessage CreateFromJson(JsonMessage jsonModel, IGatewayClientCache cache, RestClient client)
    {
        if (jsonModel.Content is null || jsonModel.Author is null)
        {
            var (guild, channel) = GetCacheData(jsonModel, cache);
            return new PartialMessage(jsonModel, guild, channel, client);
        }

        return Message.CreateFromJson(jsonModel, cache, client);
    }

    internal static (Guild?, TextChannel?) GetCacheData(JsonMessage jsonModel, IGatewayClientCache cache)
    {
        Guild? guild;
        TextChannel? channel;
        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
        {
            if (cache.Guilds.TryGetValue(guildId.GetValueOrDefault(), out guild))
            {
                var channelId = jsonModel.ChannelId;
                if (guild.Channels.TryGetValue(channelId, out var guildChannel))
                    channel = (TextChannel)guildChannel;
                else if (guild.ActiveThreads.TryGetValue(channelId, out var thread))
                    channel = thread;
                else
                    channel = null;
            }
            else
                channel = null;
        }
        else
        {
            guild = null;
            channel = cache.DMChannels.GetValueOrDefault(jsonModel.ChannelId);
        }

        return (guild, channel);
    }

    /// <summary>
    /// The ID of the <see cref="Gateway.Guild"/> the message belongs to.
    /// </summary>
    public ulong? GuildId { get; }

    /// <summary>
    /// The <see cref="Gateway.Guild"/> the message belongs to.
    /// </summary>
    public Guild? Guild { get; }

    /// <summary>
    /// The <see cref="TextChannel"/> the message was sent in.
    /// </summary>
    public TextChannel? Channel { get; }

    /// <inheritdoc cref="RestMessage.ChannelId"/>
    public ulong ChannelId { get; }

    /// <inheritdoc cref="RestMessage.Author"/>
    public User? Author { get; }

    /// <inheritdoc cref="RestMessage.Content"/>
    public string? Content { get; }

    /// <inheritdoc cref="RestMessage.EditedAt"/>
    public DateTimeOffset? EditedAt { get; }

    /// <inheritdoc cref="RestMessage.IsTts"/>
    public bool? IsTts { get; }

    /// <inheritdoc cref="RestMessage.MentionEveryone"/>
    public bool? MentionEveryone { get; }

    /// <inheritdoc cref="RestMessage.MentionedUsers"/>
    public IReadOnlyDictionary<ulong, User>? MentionedUsers { get; }

    /// <inheritdoc cref="RestMessage.MentionedRoleIds"/>
    public IReadOnlyList<ulong>? MentionedRoleIds { get; }

    /// <inheritdoc cref="RestMessage.MentionedChannels"/>
    public IReadOnlyDictionary<ulong, GuildChannelMention>? MentionedChannels { get; }

    /// <inheritdoc cref="RestMessage.Attachments"/>
    public IReadOnlyDictionary<ulong, Attachment>? Attachments { get; }

    /// <inheritdoc cref="RestMessage.Embeds"/>
    public IReadOnlyList<Embed>? Embeds { get; }

    /// <inheritdoc cref="RestMessage.Reactions"/>
    public IReadOnlyList<MessageReaction>? Reactions { get; }

    /// <inheritdoc cref="RestMessage.Nonce"/>
    public string? Nonce { get; }

    /// <inheritdoc cref="RestMessage.IsPinned"/>
    public bool? IsPinned { get; }

    /// <inheritdoc cref="RestMessage.WebhookId"/>
    public ulong? WebhookId { get; }

    /// <inheritdoc cref="RestMessage.Type"/>
    public MessageType? Type { get; }

    /// <inheritdoc cref="RestMessage.Activity"/>
    public MessageActivity? Activity { get; }

    /// <inheritdoc cref="RestMessage.Application"/>
    public Application? Application { get; }

    /// <inheritdoc cref="RestMessage.ApplicationId"/>
    public ulong? ApplicationId { get; }

    /// <inheritdoc cref="RestMessage.MessageReference"/>
    public MessageReference? MessageReference { get; }

    /// <inheritdoc cref="RestMessage.Flags"/>
    public MessageFlags? Flags { get; }

    /// <inheritdoc cref="RestMessage.ReferencedMessage"/>
    public RestMessage? ReferencedMessage { get; }

    /// <inheritdoc cref="RestMessage.InteractionMetadata"/>
    public MessageInteractionMetadata? InteractionMetadata { get; }

    /// <inheritdoc cref="RestMessage.Interaction"/>
    [Obsolete($"Replaced by '{nameof(InteractionMetadata)}'")]
    public MessageInteraction? Interaction { get; }

    /// <inheritdoc cref="RestMessage.StartedThread"/>
    public GuildThread? StartedThread { get; }

    /// <inheritdoc cref="RestMessage.Components"/>
    public IReadOnlyList<IMessageComponent>? Components { get; }

    /// <inheritdoc cref="RestMessage.Stickers"/>
    public IReadOnlyDictionary<ulong, MessageSticker>? Stickers { get; }

    /// <inheritdoc cref="RestMessage.Position"/>
    public int? Position { get; }

    /// <inheritdoc cref="RestMessage.RoleSubscriptionData"/>
    public RoleSubscriptionData? RoleSubscriptionData { get; }

    /// <inheritdoc cref="RestMessage.ResolvedData"/>
    public InteractionResolvedData? ResolvedData { get; }

    public MessagePoll? Poll { get; }

    /// <inheritdoc cref="RestMessage.ReplyAsync(ReplyMessageProperties, RestRequestProperties?)"/>
    public Task<RestMessage> ReplyAsync(ReplyMessageProperties replyMessage, RestRequestProperties? properties = null);
}

internal partial class PartialMessage : ClientEntity, IPartialMessage, IJsonModel<JsonMessage>
{
    private readonly JsonMessage _jsonModel;
    JsonMessage IJsonModel<JsonMessage>.JsonModel => _jsonModel;

    public PartialMessage(JsonMessage jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        Guild = guild;
        Channel = channel;

        var author = jsonModel.Author;
        if (author is not null)
        {
            var guildUser = jsonModel.GuildUser;
            if (guildUser is null)
                Author = new(jsonModel.Author!, client);
            else
            {
                guildUser.User = jsonModel.Author!;
                Author = new GuildUser(guildUser, jsonModel.GuildId.GetValueOrDefault(), client);
            }
        }

        var mentionedUsers = jsonModel.MentionedUsers;
        if (mentionedUsers is not null)
            MentionedUsers = mentionedUsers.ToDictionary(u => u.Id, u =>
            {
                var guildUser = u.GuildUser;
                if (guildUser is null)
                    return new User(u, client);

                guildUser.User = u;
                return new GuildUser(guildUser, jsonModel.GuildId.GetValueOrDefault(), client);
            });

        var mentionedChannels = jsonModel.MentionedChannels;
        if (mentionedChannels is not null)
            MentionedChannels = mentionedChannels.ToDictionary(c => c.Id, c => new GuildChannelMention(c));

        var attachments = jsonModel.Attachments;
        if (attachments is not null)
            Attachments = attachments.ToDictionary(a => a.Id, Attachment.CreateFromJson);

        var embeds = jsonModel.Embeds;
        if (embeds is not null)
            Embeds = embeds.Select(e => new Embed(e)).ToArray();

        var reactions = jsonModel.Reactions;
        if (reactions is not null)
            Reactions = reactions.Select(r => new MessageReaction(r)).ToArray();

        var activity = jsonModel.Activity;
        if (activity is not null)
            Activity = new(activity);

        var application = jsonModel.Application;
        if (application is not null)
            Application = new(application, client);

        var messageReference = jsonModel.MessageReference;
        if (messageReference is not null)
            MessageReference = new(messageReference);

        var referencedMessage = jsonModel.ReferencedMessage;
        if (referencedMessage is not null)
            ReferencedMessage = new(referencedMessage, client);

        var interactionMetadata = jsonModel.InteractionMetadata;
        if (interactionMetadata is not null)
            InteractionMetadata = new(interactionMetadata, client);

#pragma warning disable CS0618 // Type or member is obsolete
        var interaction = jsonModel.Interaction;
        if (interaction is not null)
            Interaction = new(interaction, client);
#pragma warning restore CS0618 // Type or member is obsolete

        var startedThread = jsonModel.StartedThread;
        if (startedThread is not null)
            StartedThread = GuildThread.CreateFromJson(startedThread, client);

        var components = jsonModel.Components;
        if (components is not null)
            Components = components.Select(IMessageComponent.CreateFromJson).ToArray();

        var stickers = jsonModel.Stickers;
        if (stickers is not null)
            Stickers = stickers.ToDictionary(s => s.Id, s => new MessageSticker(s, client));

        var roleSubscriptionData = jsonModel.RoleSubscriptionData;
        if (roleSubscriptionData is not null)
            RoleSubscriptionData = new(roleSubscriptionData);

        var resolvedData = jsonModel.ResolvedData;
        if (resolvedData is not null)
            ResolvedData = new(resolvedData, jsonModel.GuildId, client);

        var poll = jsonModel.Poll;
        if (poll is not null)
            Poll = new(poll);
    }

    public override ulong Id => _jsonModel.Id;

    public ulong? GuildId => _jsonModel.GuildId;

    public Guild? Guild { get; }

    public ulong ChannelId => _jsonModel.ChannelId;

    public TextChannel? Channel { get; }

    public User? Author { get; }

    public string? Content => _jsonModel.Content;

    public DateTimeOffset? EditedAt => _jsonModel.EditedAt;

    public bool? IsTts => _jsonModel.IsTts;

    public bool? MentionEveryone => _jsonModel.MentionEveryone;

    public IReadOnlyDictionary<ulong, User>? MentionedUsers { get; }

    public IReadOnlyList<ulong>? MentionedRoleIds => _jsonModel.MentionedRoleIds;

    public IReadOnlyDictionary<ulong, GuildChannelMention>? MentionedChannels { get; }

    public IReadOnlyDictionary<ulong, Attachment>? Attachments { get; }

    public IReadOnlyList<Embed>? Embeds { get; }

    public IReadOnlyList<MessageReaction>? Reactions { get; }

    public string? Nonce => _jsonModel.Nonce;

    public bool? IsPinned => _jsonModel.IsPinned;

    public ulong? WebhookId => _jsonModel.WebhookId;

    public MessageType? Type => _jsonModel.Type;

    public MessageActivity? Activity { get; }

    public Application? Application { get; }

    public ulong? ApplicationId => _jsonModel.ApplicationId;

    public MessageReference? MessageReference { get; }

    public MessageFlags? Flags => _jsonModel.Flags;

    public RestMessage? ReferencedMessage { get; }

    public MessageInteractionMetadata? InteractionMetadata { get; }

    public MessageInteraction? Interaction { get; }

    public GuildThread? StartedThread { get; }

    public IReadOnlyList<IMessageComponent>? Components { get; }

    public IReadOnlyDictionary<ulong, MessageSticker>? Stickers { get; }

    public int? Position => _jsonModel.Position;

    public RoleSubscriptionData? RoleSubscriptionData { get; }

    public InteractionResolvedData? ResolvedData { get; }

    public MessagePoll? Poll { get; }

    public Task<RestMessage> ReplyAsync(ReplyMessageProperties replyMessage, RestRequestProperties? properties = null)
        => SendAsync(replyMessage.ToMessageProperties(Id), properties);
}
