namespace NetCord.Rest;

public class RestMessage : ClientEntity, IJsonModel<JsonModels.JsonMessage>
{
    JsonModels.JsonMessage IJsonModel<JsonModels.JsonMessage>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonMessage _jsonModel;

    public override ulong Id => _jsonModel.Id;
    public ulong ChannelId => _jsonModel.ChannelId;
    public User Author { get; }
    public string Content => _jsonModel.Content;
    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;
    public DateTimeOffset? EditedAt => _jsonModel.EditedAt;
    public bool IsTts => _jsonModel.IsTts;
    public bool MentionEveryone => _jsonModel.MentionEveryone;
    public IReadOnlyDictionary<ulong, User> MentionedUsers { get; }
    public IReadOnlyList<ulong> MentionedRoleIds { get; }
    public IReadOnlyDictionary<ulong, GuildChannelMention> MentionedChannels { get; }
    public IReadOnlyDictionary<ulong, Attachment> Attachments { get; }
    public IEnumerable<Embed> Embeds { get; }
    public IEnumerable<MessageReaction> Reactions { get; }
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
    public IEnumerable<IComponent> Components { get; }
    public IReadOnlyDictionary<ulong, MessageSticker> Stickers { get; }
    public int? Position => _jsonModel.Position;
    public RoleSubscriptionData? RoleSubscriptionData { get; }

    public RestMessage(JsonModels.JsonMessage jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        if (jsonModel.GuildUser == null)
        {
            Author = new(jsonModel.Author, client);
            MentionedUsers = jsonModel.MentionedUsers.ToDictionary(u => u.Id, u => new User(u, client));
        }
        else
        {
            var guildId = jsonModel.GuildId.GetValueOrDefault();
            jsonModel.GuildUser.User = jsonModel.Author;
            Author = new GuildUser(jsonModel.GuildUser, guildId, client);
            MentionedUsers = jsonModel.MentionedUsers.ToDictionary(u => u.Id, u =>
            {
                u.GuildUser!.User = u;
                return (User)new GuildUser(u.GuildUser, guildId, client);
            });
        }

        MentionedRoleIds = jsonModel.MentionedRoleIds;
        MentionedChannels = jsonModel.MentionedChannels.ToDictionaryOrEmpty(c => c.Id, c => new GuildChannelMention(c));
        Attachments = jsonModel.Attachments.ToDictionary(a => a.Id, Attachment.CreateFromJson);
        Embeds = jsonModel.Embeds.Select(e => new Embed(e));
        Reactions = jsonModel.Reactions.SelectOrEmpty(r => new MessageReaction(r, client));

        if (jsonModel.Activity != null)
            Activity = new(jsonModel.Activity);
        if (jsonModel.Application != null)
            Application = new(jsonModel.Application, client);
        if (jsonModel.MessageReference != null)
            MessageReference = new(jsonModel.MessageReference);
        if (jsonModel.ReferencedMessage != null)
            ReferencedMessage = new(jsonModel.ReferencedMessage, client);
        if (jsonModel.Interaction != null)
            Interaction = new(jsonModel.Interaction, client);
        if (jsonModel.StartedThread != null)
            StartedThread = (GuildThread)Channel.CreateFromJson(jsonModel.StartedThread, client);
        Components = jsonModel.Components.SelectOrEmpty(IComponent.CreateFromJson);
        Stickers = jsonModel.Stickers.ToDictionaryOrEmpty(s => s.Id, s => new MessageSticker(s, client));
        if (jsonModel.RoleSubscriptionData != null)
            RoleSubscriptionData = new(jsonModel.RoleSubscriptionData);
    }

    public Task<RestMessage> ReplyAsync(string content, bool replyMention = false, bool failIfNotExists = true)
    {
        MessageProperties message = new()
        {
            Content = content,
            MessageReference = new(Id, failIfNotExists),
            AllowedMentions = new()
            {
                ReplyMention = replyMention
            },
        };
        return _client.SendMessageAsync(ChannelId, message);
    }

    #region Channel
    public Task<RestMessage> CrosspostAsync(RequestProperties? properties = null) => _client.CrosspostMessageAsync(ChannelId, Id, properties);
    public Task<RestMessage> ModifyAsync(Action<MessageOptions> action, RequestProperties? properties = null) => _client.ModifyMessageAsync(ChannelId, Id, action, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteMessageAsync(ChannelId, Id, properties);
    public Task AddReactionAsync(ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.AddMessageReactionAsync(ChannelId, Id, emoji, properties);
    public Task DeleteReactionAsync(ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.DeleteMessageReactionAsync(ChannelId, Id, emoji, properties);
    public Task DeleteReactionAsync(ReactionEmojiProperties emoji, ulong userId, RequestProperties? properties = null) => _client.DeleteMessageReactionAsync(ChannelId, Id, emoji, userId, properties);
    public IAsyncEnumerable<User> GetReactionsAsync(ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.GetMessageReactionsAsync(ChannelId, Id, emoji, properties);
    public IAsyncEnumerable<User> GetReactionsAfterAsync(ReactionEmojiProperties emoji, ulong userId, RequestProperties? properties = null) => _client.GetMessageReactionsAfterAsync(ChannelId, Id, emoji, userId, properties);
    public Task DeleteAllReactionsAsync(ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.DeleteAllMessageReactionsAsync(ChannelId, Id, emoji, properties);
    public Task DeleteAllReactionsAsync(RequestProperties? properties = null) => _client.DeleteAllMessageReactionsAsync(ChannelId, Id, properties);
    #endregion
}
