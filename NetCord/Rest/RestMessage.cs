namespace NetCord.Rest;

public partial class RestMessage : ClientEntity, IJsonModel<NetCord.JsonModels.JsonMessage>
{
    NetCord.JsonModels.JsonMessage IJsonModel<NetCord.JsonModels.JsonMessage>.JsonModel => _jsonModel;
    private protected readonly NetCord.JsonModels.JsonMessage _jsonModel;

    public RestMessage(NetCord.JsonModels.JsonMessage jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var guildUser = jsonModel.GuildUser;
        if (guildUser is null)
        {
            Author = new(jsonModel.Author, client);
            MentionedUsers = jsonModel.MentionedUsers.ToDictionary(u => u.Id, u => new User(u, client));
        }
        else
        {
            var guildId = jsonModel.GuildId.GetValueOrDefault();
            guildUser.User = jsonModel.Author;
            Author = new GuildUser(guildUser, guildId, client);
            MentionedUsers = jsonModel.MentionedUsers.ToDictionary(u => u.Id, u =>
            {
                var guildUser = u.GuildUser!;
                guildUser.User = u;
                return (User)new GuildUser(guildUser, guildId, client);
            });
        }

        MentionedChannels = jsonModel.MentionedChannels.ToDictionaryOrEmpty(c => c.Id, c => new GuildChannelMention(c));
        Attachments = jsonModel.Attachments.ToDictionary(a => a.Id, Attachment.CreateFromJson);
        Embeds = jsonModel.Embeds.Select(e => new Embed(e)).ToArray();
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

        var interaction = jsonModel.Interaction;
        if (interaction is not null)
            Interaction = new(interaction, client);

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

    public override ulong Id => _jsonModel.Id;
    public ulong ChannelId => _jsonModel.ChannelId;
    public User Author { get; }
    public string Content => _jsonModel.Content;
    //public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;
    public DateTimeOffset? EditedAt => _jsonModel.EditedAt;
    public bool IsTts => _jsonModel.IsTts;
    public bool MentionEveryone => _jsonModel.MentionEveryone;
    public IReadOnlyDictionary<ulong, User> MentionedUsers { get; }
    public IReadOnlyList<ulong> MentionedRoleIds => _jsonModel.MentionedRoleIds;
    public IReadOnlyDictionary<ulong, GuildChannelMention> MentionedChannels { get; }
    public IReadOnlyDictionary<ulong, Attachment> Attachments { get; }
    public IReadOnlyList<Embed> Embeds { get; }
    public IReadOnlyList<MessageReaction> Reactions { get; }
    public string? Nonce => _jsonModel.Nonce;
    public bool IsPinned => _jsonModel.IsPinned;
    public ulong? WebhookId => _jsonModel.WebhookId;
    public MessageType Type => _jsonModel.Type;
    public MessageActivity? Activity { get; }
    public Application? Application { get; }
    public ulong? ApplicationId => _jsonModel.ApplicationId;
    public MessageReference? MessageReference { get; }
    public MessageFlags Flags => _jsonModel.Flags.GetValueOrDefault();
    public RestMessage? ReferencedMessage { get; }
    public MessageInteraction? Interaction { get; }
    public GuildThread? StartedThread { get; }
    public IReadOnlyList<IMessageComponent> Components { get; }
    public IReadOnlyDictionary<ulong, MessageSticker> Stickers { get; }
    public int? Position => _jsonModel.Position;
    public RoleSubscriptionData? RoleSubscriptionData { get; }
    public InteractionResolvedData? ResolvedData { get; }

    public Task<RestMessage> ReplyAsync(ReplyMessageProperties replyMessage, RequestProperties? properties = null)
        => SendAsync(replyMessage.ToMessageProperties(Id), properties);
}
