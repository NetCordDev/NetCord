namespace NetCord.Rest;

public class RestMessage : WebhookMessage
{
    public virtual User Author { get; }

    public string Content => _jsonModel.Content;

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public DateTimeOffset? EditedAt => _jsonModel.EditedAt;

    public bool IsTts => _jsonModel.IsTts;

    public bool MentionEveryone => _jsonModel.MentionEveryone;

    public IReadOnlyDictionary<ulong, User> MentionedUsers { get; }

    public IEnumerable<ulong> MentionedRoleIds { get; }

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

    public MessageReference? MessageReference { get; }

    public MessageFlags Flags => _jsonModel.Flags.GetValueOrDefault();

    public RestMessage? ReferencedMessage { get; }

    public MessageInteraction? Interaction { get; }

    public IReadOnlyDictionary<ulong, MessageSticker> Stickers { get; }

    public GuildThread? StartedThread { get; }

    public RestMessage(JsonModels.JsonMessage jsonModel, RestClient client) : base(jsonModel, client)
    {
        if (jsonModel.GuildUser == null)
            Author = new(jsonModel.Author, client);
        else
        {
            jsonModel.GuildUser.User = jsonModel.Author;
            Author = new GuildUser(jsonModel.GuildUser, GuildId.GetValueOrDefault(), client);
        }

        MentionedUsers = jsonModel.MentionedUsers!.ToDictionary(u => u.Id, u => new User(u, client));
        MentionedRoleIds = jsonModel.MentionedRoleIds!;
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

        Stickers = jsonModel.Stickers.ToDictionaryOrEmpty(s => s.Id, s => new MessageSticker(s, client));
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
            }
        };
        return _client.SendMessageAsync(ChannelId, message);
    }

    #region Channel
    public Task<RestMessage> ModifyAsync(Action<MessageOptions> action, RequestProperties? properties = null) => _client.ModifyMessageAsync(ChannelId, Id, action, properties);
    #endregion
}
