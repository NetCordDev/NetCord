using NetCord.JsonModels;

namespace NetCord;

public abstract class Interaction : ClientEntity
{
    internal JsonInteraction _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public DiscordId ApplicationId => _jsonEntity.ApplicationId;

    public InteractionType Type => _jsonEntity.Type;

    //public DiscordId? GuildId => _jsonEntity.GuildId;
    public Guild? Guild { get; }

    //public DiscordId? Channel => Message.Channel;

    public DiscordId? ChannelId => _jsonEntity.ChannelId;

    public User User { get; }

    public string Token => _jsonEntity.Token;

    public UserMessage Message { get; }

    public abstract ButtonInteractionData Data { get; }

    internal Interaction(JsonInteraction jsonEntity, SocketClient client) : base(client.Rest)
    {
        _jsonEntity = jsonEntity;
        var guildId = jsonEntity.GuildId;
        if (guildId.HasValue && client.Guilds.TryGetValue(guildId.GetValueOrDefault(), out Guild? guild))
        {
            Guild = guild;
            User = new GuildUser(jsonEntity.GuildUser!, Guild, client.Rest);
            Message = new(jsonEntity.Message with { GuildId = jsonEntity.GuildId }, client);
        }
        else
        {
            User = new User(jsonEntity.User!, client.Rest);
            Message = new(jsonEntity.Message, client);
        }
    }

    public Task EndAsync(RequestOptions? options = null) => _client.Interaction.EndAsync(Id, Token, options);

    public Task EndWithModifyAsync(InteractionMessage message, RequestOptions? options = null) => _client.Interaction.EndWithModifyAsync(Id, Token, message, options);

    public Task EndWithReplyAsync(InteractionMessage message, RequestOptions? options = null) => _client.Interaction.EndWithReplyAsync(Id, Token, message, options);

    public Task EndWithThinkingStateAsync(RequestOptions? options = null) => _client.Interaction.EndWithThinkingStateAsync(Id, Token, options);

    public Task ModifyThinkingStateAsync(Message message, RequestOptions? options = null) => _client.Interaction.ModifyThinkingStateAsync(ApplicationId, Token, message, options);

    public Task ModifyMessageAsync(Message message, RequestOptions? options = null) => _client.Interaction.ModifyMessageAsync(ApplicationId, Token, Message.Id, message, options);
}