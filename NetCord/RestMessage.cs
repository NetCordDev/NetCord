namespace NetCord;

public class RestMessage : ClientEntity
{
    private protected readonly JsonModels.JsonMessage _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public DiscordId ChannelId => _jsonEntity.ChannelId;

    public virtual User Author { get; }

    public string Content => _jsonEntity.Content;

    public DateTimeOffset CreatedAt => _jsonEntity.CreatedAt;

    public DateTimeOffset? EditedAt => _jsonEntity.EditedAt;

    public bool IsTts => _jsonEntity.IsTts;

    public bool MentionEveryone => _jsonEntity.MentionEveryone;

    public IReadOnlyDictionary<DiscordId, User> MentionedUsers { get; }

    public IEnumerable<DiscordId> MentionedRolesIds { get; }

    public IReadOnlyDictionary<DiscordId, GuildChannelMention> MentionedChannels { get; }

    public IReadOnlyDictionary<DiscordId, Attachment> Attachments { get; }

    public IEnumerable<Embed> Embeds { get; }

    public IEnumerable<MessageReaction> Reactions { get; }

    public string? Nonce => _jsonEntity.Nonce;

    public bool IsPinned => _jsonEntity.IsPinned;

    public DiscordId? WebhookId => _jsonEntity.WebhookId;

    public MessageType Type => _jsonEntity.Type;

    public MessageActivity? Activity { get; }

    public Application? Application { get; }

    public DiscordId? ApplicationId => _jsonEntity.ApplicationId;

    public Reference? MessageReference { get; }

    public MessageFlags? Flags => _jsonEntity.Flags;

    public RestMessage? ReferencedMessage { get; }

    public MessageInteraction? Interaction { get; }

    public IEnumerable<IComponent> Components { get; }

    public IReadOnlyDictionary<DiscordId, MessageSticker> Stickers { get; }

    public Thread? StartedThread { get; }

    internal RestMessage(JsonModels.JsonMessage jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;

        if (jsonEntity.Member == null)
            Author = new(jsonEntity.Author, client);
        else
            Author = new GuildUser(jsonEntity.Member with { User = jsonEntity.Author }, jsonEntity.GuildId.GetValueOrDefault(), client);

        MentionedUsers = jsonEntity.MentionedUsers.ToDictionary(u => u.Id, u => new User(u, client));
        MentionedRolesIds = jsonEntity.MentionedRoles;
        MentionedChannels = jsonEntity.MentionedChannels.ToDictionaryOrEmpty(c => c.Id, c => new GuildChannelMention(c));
        Attachments = jsonEntity.Attachments.ToDictionary(a => a.Id, a => Attachment.CreateFromJson(a));
        Embeds = jsonEntity.Embeds.Select(e => new Embed(e));
        Reactions = jsonEntity.Reactions.SelectOrEmpty(r => new MessageReaction(r, client));

        if (jsonEntity.Activity != null)
            Activity = new(jsonEntity.Activity);
        if (jsonEntity.Application != null)
            Application = new(jsonEntity.Application, client);
        if (jsonEntity.MessageReference != null)
            MessageReference = new(jsonEntity.MessageReference);
        if (jsonEntity.ReferencedMessage != null)
            ReferencedMessage = new(jsonEntity.ReferencedMessage, client);
        if (jsonEntity.Interaction != null)
            Interaction = new(jsonEntity.Interaction, client);
        if (jsonEntity.StartedThread != null)
            StartedThread = (Thread)Channel.CreateFromJson(jsonEntity.StartedThread, client);
        if (jsonEntity.MessageReference != null)
            MessageReference = new(jsonEntity.MessageReference);

        Components = jsonEntity.Components.Select(c => IComponent.CreateFromJson(c));
        Stickers = jsonEntity.Stickers.ToDictionaryOrEmpty(s => s.Id, s => new MessageSticker(s, client));

    }

    public Task AddReactionAsync(ReactionEmoji emoji, RequestOptions? options = null) => _client.Message.AddReactionAsync(ChannelId, Id, emoji, options);

    public Task DeleteReactionAsync(ReactionEmoji emoji, DiscordId userId, RequestOptions? options = null) => _client.Message.DeleteReactionAsync(ChannelId, Id, emoji, userId, options);
    public Task DeleteAllReactionsAsync(ReactionEmoji emoji, RequestOptions? options = null) => _client.Message.DeleteAllReactionsAsync(ChannelId, Id, emoji, options);
    public Task DeleteAllReactionsAsync(RequestOptions? options = null) => _client.Message.DeleteAllReactionsAsync(ChannelId, Id, options);

    public Task DeleteAsync(RequestOptions? options = null) => _client.Message.DeleteAsync(ChannelId, Id, options);

    public virtual string GetJumpUrl(DiscordId? guildId) => $"https://discord.com/channels/{(guildId != null ? guildId : "@me")}/{ChannelId}/{Id}";

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
        return _client.Message.SendAsync(ChannelId, message);
    }
}