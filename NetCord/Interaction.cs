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

    public User? User { get; }

    public string Token => _jsonEntity.Token;

    public UserMessage Message { get; }

    public abstract ButtonInteractionData Data { get; }

    internal Interaction(JsonInteraction jsonEntity, BotClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
        if (client.TryGetGuild(jsonEntity.GuildId, out Guild guild))
        {
            Guild = guild;
            User = new GuildUser(jsonEntity.GuildUser, client, Guild);
        }
        else
            User = new User(jsonEntity.User, client);
        Message = new(jsonEntity.Message with { GuildId = jsonEntity.GuildId }, client);
    }

    public Task EndAsync() => InteractionHelper.EndInteractionAsync(_client, Id, Token);

    public Task EndWithModifyAsync(InteractionMessage message) => InteractionHelper.EndInteractionWithModifyAsync(_client, Id, Token, message);

    public Task EndWithReplyAsync(InteractionMessage message) => InteractionHelper.EndInteractionWithReplyAsync(_client, Id, Token, message);

    public Task EndWithThinkingStateAsync() => InteractionHelper.EndInteractionWithThinkingStateAsync(_client, Id, Token);

    public Task ModifyThinkingStateAsync(BuiltMessage message) => InteractionHelper.ModifyThinkingStateAsync(_client, ApplicationId, Token, message);

    public Task ModifyMessageAsync(BuiltMessage message) => InteractionHelper.ModifyInteractionMessageAsync(_client, ApplicationId, Token, Message.Id, message);
}