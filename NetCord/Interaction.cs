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

    public Message Message { get; }

    public abstract ButtonInteractionData Data { get; }

    internal Interaction(JsonInteraction jsonEntity, BotClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
        var guildId = jsonEntity.GuildId;
        if (guildId != null && client.Guilds.TryGetValue(guildId, out Guild? guild))
        {
            Guild = guild;
            User = new GuildUser(jsonEntity.GuildUser!, Guild, client);
            Message = new(jsonEntity.Message with { GuildId = jsonEntity.GuildId }, client);
        }
        else
        {
            User = new User(jsonEntity.User!, client);
            Message = new(jsonEntity.Message, client);
        }
    }

    public Task EndAsync(RequestOptions? options = null) => _client.Rest.Interaction.EndAsync(Id, Token, options);

    public Task EndWithModifyAsync(InteractionMessage message, RequestOptions? options = null) => _client.Rest.Interaction.EndWithModifyAsync(Id, Token, message, options);

    public Task EndWithReplyAsync(InteractionMessage message, RequestOptions? options = null) => _client.Rest.Interaction.EndWithReplyAsync(Id, Token, message, options);

    public Task EndWithThinkingStateAsync(RequestOptions? options = null) => _client.Rest.Interaction.EndWithThinkingStateAsync(Id, Token, options);

    public Task ModifyThinkingStateAsync(BuiltMessage message, RequestOptions? options = null) => _client.Rest.Interaction.ModifyThinkingStateAsync(ApplicationId, Token, message, options);

    public Task ModifyMessageAsync(BuiltMessage message, RequestOptions? options = null) => _client.Rest.Interaction.ModifyMessageAsync(ApplicationId, Token, Message.Id, message, options);
}