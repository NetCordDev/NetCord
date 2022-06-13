namespace NetCord;

public class RestMessage : ClientEntity, IJsonModel<JsonModels.JsonMessage>
{
    JsonModels.JsonMessage IJsonModel<JsonModels.JsonMessage>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonMessage _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public virtual User Author { get; }

    public string Content => _jsonModel.Content;

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public DateTimeOffset? EditedAt => _jsonModel.EditedAt;

    public bool IsTts => _jsonModel.IsTts;

    public bool MentionEveryone => _jsonModel.MentionEveryone;

    public IReadOnlyDictionary<Snowflake, User> MentionedUsers { get; }

    public IEnumerable<Snowflake> MentionedRoleIds { get; }

    public IReadOnlyDictionary<Snowflake, GuildChannelMention> MentionedChannels { get; }

    public IReadOnlyDictionary<Snowflake, Attachment> Attachments { get; }

    public IEnumerable<Embed> Embeds { get; }

    public IEnumerable<MessageReaction> Reactions { get; }

    public string? Nonce => _jsonModel.Nonce;

    public bool IsPinned => _jsonModel.IsPinned;

    public Snowflake? WebhookId => _jsonModel.WebhookId;

    public MessageType Type => _jsonModel.Type;

    public MessageActivity? Activity { get; }

    public Application? Application { get; }

    public Snowflake? ApplicationId => _jsonModel.ApplicationId;

    public Reference? MessageReference { get; }

    public MessageFlags? Flags => _jsonModel.Flags;

    public RestMessage? ReferencedMessage { get; }

    public MessageInteraction? Interaction { get; }

    public IEnumerable<IComponent> Components { get; }

    public IReadOnlyDictionary<Snowflake, MessageSticker> Stickers { get; }

    public GuildThread? StartedThread { get; }

    public RestMessage(JsonModels.JsonMessage jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        if (jsonModel.Member == null)
            Author = new(jsonModel.Author, client);
        else
            Author = new GuildUser(jsonModel.Member with { User = jsonModel.Author }, jsonModel.GuildId.GetValueOrDefault(), client);

        MentionedUsers = jsonModel.MentionedUsers.ToDictionary(u => u.Id, u => new User(u, client));
        MentionedRoleIds = jsonModel.MentionedRoleIds;
        MentionedChannels = jsonModel.MentionedChannels.ToDictionaryOrEmpty(c => c.Id, c => new GuildChannelMention(c));
        Attachments = jsonModel.Attachments.ToDictionary(a => a.Id, a => Attachment.CreateFromJson(a));
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
        if (jsonModel.MessageReference != null)
            MessageReference = new(jsonModel.MessageReference);

        Components = jsonModel.Components.Select(IComponent.CreateFromJson);
        Stickers = jsonModel.Stickers.ToDictionaryOrEmpty(s => s.Id, s => new MessageSticker(s, client));
    }

    public Task AddReactionAsync(ReactionEmojiProperties emoji, RequestProperties? options = null) => _client.AddMessageReactionAsync(ChannelId, Id, emoji, options);

    public Task DeleteReactionAsync(ReactionEmojiProperties emoji, Snowflake userId, RequestProperties? options = null) => _client.DeleteMessageReactionAsync(ChannelId, Id, emoji, userId, options);
    public Task DeleteAllReactionsAsync(ReactionEmojiProperties emoji, RequestProperties? options = null) => _client.DeleteAllMessageReactionsAsync(ChannelId, Id, emoji, options);
    public Task DeleteAllReactionsAsync(RequestProperties? options = null) => _client.DeleteAllMessageReactionsAsync(ChannelId, Id, options);

    public Task DeleteAsync(RequestProperties? options = null) => _client.DeleteMessageAsync(ChannelId, Id, options);

    public virtual string GetJumpUrl(Snowflake? guildId) => $"https://discord.com/channels/{(guildId != null ? guildId : "@me")}/{ChannelId}/{Id}";

    public Task<RestMessage> ReplyAsync(string content, bool replyMention = false, bool failIfNotExists = true)
    {
        MessageProperties message = new()
        {
            Content = content,
            MessageReference = new(this, failIfNotExists),
            AllowedMentions = new()
            {
                ReplyMention = replyMention
            }
        };
        return _client.SendMessageAsync(ChannelId, message);
    }
}