﻿namespace NetCord.Rest;

/// <summary>
/// Represents a message sent in a channel within Discord.
/// </summary>
public partial class RestMessage : ClientEntity, IJsonModel<NetCord.JsonModels.JsonMessage>
{
    NetCord.JsonModels.JsonMessage IJsonModel<NetCord.JsonModels.JsonMessage>.JsonModel => _jsonModel;
    private protected readonly NetCord.JsonModels.JsonMessage _jsonModel;

    public RestMessage(NetCord.JsonModels.JsonMessage jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var guildUser = jsonModel.GuildUser;
        if (guildUser is null)
            Author = new(jsonModel.Author!, client);
        else
        {
            guildUser.User = jsonModel.Author!;
            Author = new GuildUser(guildUser, jsonModel.GuildId.GetValueOrDefault(), client);
        }

        MentionedUsers = jsonModel.MentionedUsers!.ToDictionary(u => u.Id, u =>
        {
            var guildUser = u.GuildUser;
            if (guildUser is null)
                return new User(u, client);

            guildUser.User = u;
            return new GuildUser(guildUser, jsonModel.GuildId.GetValueOrDefault(), client);
        });

        MentionedChannels = jsonModel.MentionedChannels.ToDictionaryOrEmpty(c => c.Id, c => new GuildChannelMention(c));
        Attachments = jsonModel.Attachments!.ToDictionary(a => a.Id, Attachment.CreateFromJson);
        Embeds = jsonModel.Embeds!.Select(e => new Embed(e)).ToArray();
        Reactions = jsonModel.Reactions.SelectOrEmpty(r => new MessageReaction(r)).ToArray();

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

        Components = jsonModel.Components.SelectOrEmpty(IMessageComponent.CreateFromJson).ToArray();
        Stickers = jsonModel.Stickers.ToDictionaryOrEmpty(s => s.Id, s => new MessageSticker(s, client));

        var roleSubscriptionData = jsonModel.RoleSubscriptionData;
        if (roleSubscriptionData is not null)
            RoleSubscriptionData = new(roleSubscriptionData);

        var resolvedData = jsonModel.ResolvedData;
        if (resolvedData is not null)
            ResolvedData = new(resolvedData, jsonModel.GuildId, client);
    }

    /// <summary>
    /// The ID of the message.
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The ID of the channel the message was sent in.
    /// </summary>
    public ulong ChannelId => _jsonModel.ChannelId;

    /// <summary>
    /// The <see cref="User"/> object of the message's author.
    /// </summary>
    /// <remarks>
    ///  The author object follows the structure of the user object, but is only a valid user in the case where the message is generated by a user or bot user. If the message is generated by a <see cref="Webhook"/>, the author object corresponds to the webhook's id, username, and avatar. You can tell if a message is generated by a webhook by checking for the <see cref="WebhookId"/> on the message object.
    /// </remarks>
    public User Author { get; }

    /// <summary>
    /// The text contents of the message.
    /// </summary>
    public string Content => _jsonModel.Content!;

    /// <summary>
    /// When the message was edited (or null if never).
    /// </summary>
    public DateTimeOffset? EditedAt => _jsonModel.EditedAt;

    /// <summary>
    /// Whether the message was a Text-To-Speech message.
    /// </summary>
    public bool IsTts => _jsonModel.IsTts.GetValueOrDefault();

    /// <summary>
    /// Whether the message mentions @everyone.
    /// </summary>
    public bool MentionEveryone => _jsonModel.MentionEveryone.GetValueOrDefault();

    /// <summary>
    /// A dictionary of <see cref="User"/> objects indexed by their IDs, containing users specifically mentioned in the message.
    /// </summary>
    public IReadOnlyDictionary<ulong, User> MentionedUsers { get; }

    /// <summary>
    /// A list of IDs corresponding to roles specifically mentioned in the message.
    /// </summary>
    public IReadOnlyList<ulong> MentionedRoleIds => _jsonModel.MentionedRoleIds!;

    /// <summary>
    /// A dictionary of <see cref="GuildChannelMention"/> objects indexed by their IDs, containing channels specifically mentioned in the message.
    /// </summary>
    /// <remarks>
    /// Not all channel mentions in a message will appear in <see cref="MentionedChannels"/>. Only <see cref="TextChannel"/>s visible to everyone in a lurkable guild will ever be included. Only crossposted messages (via Channel Following) currently include <see cref="MentionedChannels"/> at all. If no <see cref="Mention"/>s in the message meet these requirements, this field will not be sent.
    /// </remarks>
    public IReadOnlyDictionary<ulong, GuildChannelMention> MentionedChannels { get; }

    /// <summary>
    /// A dictionary of <see cref="Attachment"/> objects indexed by their IDs, containing any files attached in the message.
    /// </summary>
    public IReadOnlyDictionary<ulong, Attachment> Attachments { get; }

    /// <summary>
    /// A list of <see cref="Embed"/> objects containing any embedded content present in the message.
    /// </summary>
    public IReadOnlyList<Embed> Embeds { get; }

    /// <summary>
    /// A list of <see cref="MessageReaction"/> objects containing all reactions to the message.
    /// </summary>
    public IReadOnlyList<MessageReaction> Reactions { get; }

    /// <summary>
    /// Used for validating that a message was sent.
    /// </summary>
    public string? Nonce => _jsonModel.Nonce;

    /// <summary>
    /// Whether this message is pinned in a channel.
    /// </summary>
    public bool IsPinned => _jsonModel.IsPinned.GetValueOrDefault();

    /// <summary>
    /// If the message was generated by a <see cref="Webhook"/>, this is the webhook's ID.
    /// </summary>
    public ulong? WebhookId => _jsonModel.WebhookId;

    /// <summary>
    /// The type of the message.
    /// </summary>
    public MessageType Type => _jsonModel.Type.GetValueOrDefault();

    /// <summary>
    /// Sent with Rich Presence-related chat embeds.
    /// </summary>
    public MessageActivity? Activity { get; }

    /// <summary>
    /// Sent with Rich Presence-related chat embeds.
    /// </summary>
    public Application? Application { get; }

    /// <summary>
    /// If the message is an <see cref="IInteraction"/> or application-owned <see cref="Webhook"/>, this is the ID of the <see cref="NetCord.Application"/>.
    /// </summary>
    public ulong? ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// Contains data showing the source of a crosspost, channel follow add, pin, or message reply.
    /// </summary>
    public MessageReference? MessageReference { get; }

    /// <summary>
    /// A <see cref="MessageFlags"/> object indicating the message's applied flags.
    /// </summary>
    public MessageFlags Flags => _jsonModel.Flags.GetValueOrDefault();

    /// <summary>
    /// The message associated with the <see cref="MessageReference"/>.
    /// </summary>
    /// <remarks>
    /// This field is only returned for messages with a <see cref="MessageType"/> of <see cref="MessageType.Reply"/> or <see cref="MessageType.ThreadStarterMessage"/>. If the message is a reply but the <see cref="ReferencedMessage"/> field is not present, the backend did not attempt to fetch the message that was being replied to, so its state is unknown. If the field exists but is null, the referenced message was deleted.
    /// </remarks>
    public RestMessage? ReferencedMessage { get; }

    /// <summary>
    /// Sent if the message is sent as a result of an <see cref="IInteraction"/>.
    /// </summary>
    public MessageInteractionMetadata? InteractionMetadata { get; }

    /// <summary>
    /// Sent if the message is a response to an <see cref="IInteraction"/>.
    /// </summary>
    [Obsolete($"Replaced by '{nameof(InteractionMetadata)}'")]
    public MessageInteraction? Interaction { get; }

    /// <summary>
    /// The <see cref="GuildThread"/> that was started from this message.
    /// </summary>
    public GuildThread? StartedThread { get; }

    /// <summary>
    /// A list of <see cref="IMessageComponent"/> objects, sent if the message contains components like <see cref="Button"/>s, <see cref="ActionRow"/>s, or other interactive components.
    /// </summary>
    public IReadOnlyList<IMessageComponent> Components { get; }

    /// <summary>
    /// Sent if the message contains stickers.
    /// </summary>
    public IReadOnlyDictionary<ulong, MessageSticker> Stickers { get; }

    /// <summary>
    /// A generally increasing integer (there may be gaps or duplicates) that represents the approximate position of the message in a <see cref="GuildThread"/>.
    /// It can be used to estimate the relative position of the message in a thread in tandem with the <see cref="GuildThread.TotalMessageSent"/> property of the parent thread.
    /// </summary>
    public int? Position => _jsonModel.Position;

    /// <summary>
    /// Data of the role subscription purchase or renewal that prompted this <see cref="MessageType.RoleSubscriptionPurchase"/> message.
    /// </summary>
    public RoleSubscriptionData? RoleSubscriptionData { get; }

    /// <summary>
    /// Data for <see cref="User"/>s, <see cref="GuildUser"/>s, <see cref="IGuildChannel"/>s, and <see cref="Role"/>s in the message's auto-populated select <see cref="Menu"/>s.
    /// </summary>
    public InteractionResolvedData? ResolvedData { get; }

    public Task<RestMessage> ReplyAsync(ReplyMessageProperties replyMessage, RestRequestProperties? properties = null)
        => SendAsync(replyMessage.ToMessageProperties(Id), properties);
}
