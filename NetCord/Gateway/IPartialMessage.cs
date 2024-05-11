using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public partial interface IPartialMessage : IEntity
{
    public static IPartialMessage CreateFromJson(JsonMessage jsonModel, IGatewayClientCache cache, RestClient client)
    {
        if (jsonModel.Author is null)
        {
            var (guild, channel) = GetCacheData(jsonModel, cache);
            return new PartialMessage(jsonModel, guild, channel, client);
        }

        return Message.CreateFromJson(jsonModel, cache, client);
    }

    public static (Guild?, TextChannel?) GetCacheData(JsonMessage jsonModel, IGatewayClientCache cache)
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

    public ulong? GuildId { get; }

    public Guild? Guild { get; }

    public TextChannel? Channel { get; }

    public ulong ChannelId { get; }

    public string? Content { get; }

    public DateTimeOffset? EditedAt { get; }

    public bool? IsTts { get; }

    public bool? MentionEveryone { get; }

    public IReadOnlyDictionary<ulong, User>? MentionedUsers { get; }

    public IReadOnlyList<ulong>? MentionedRoleIds { get; }

    public IReadOnlyDictionary<ulong, GuildChannelMention>? MentionedChannels { get; }

    public IReadOnlyDictionary<ulong, Attachment>? Attachments { get; }

    public IReadOnlyList<Embed>? Embeds { get; }

    public IReadOnlyList<MessageReaction>? Reactions { get; }

    public string? Nonce { get; }

    public bool? IsPinned { get; }

    public ulong? WebhookId { get; }

    public MessageType? Type { get; }

    public MessageActivity? Activity { get; }

    public Application? Application { get; }

    public ulong? ApplicationId { get; }

    public MessageReference? MessageReference { get; }

    public MessageFlags? Flags { get; }

    public RestMessage? ReferencedMessage { get; }

    public MessageInteractionMetadata? InteractionMetadata { get; }

    [Obsolete($"Replaced by '{nameof(InteractionMetadata)}'")]
    public MessageInteraction? Interaction { get; }

    public GuildThread? StartedThread { get; }

    public IReadOnlyList<IMessageComponent>? Components { get; }

    public IReadOnlyDictionary<ulong, MessageSticker>? Stickers { get; }

    public int? Position { get; }

    public RoleSubscriptionData? RoleSubscriptionData { get; }

    public InteractionResolvedData? ResolvedData { get; }

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
    }

    public override ulong Id => _jsonModel.Id;

    public ulong? GuildId => _jsonModel.GuildId;

    public Guild? Guild { get; }

    public ulong ChannelId => _jsonModel.ChannelId;

    public TextChannel? Channel { get; }

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

    public Task<RestMessage> ReplyAsync(ReplyMessageProperties replyMessage, RestRequestProperties? properties = null)
        => SendAsync(replyMessage.ToMessageProperties(Id), properties);
}
