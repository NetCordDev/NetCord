namespace NetCord;

public class MessageReaction : ClientEntity
{
    private readonly JsonModels.JsonMessageReaction _jsonEntity;

    public int Count => _jsonEntity.Count;

    public bool Me => _jsonEntity.Me;

    public override DiscordId? Id => _jsonEntity.Emoji.Id;

    public string? Name => _jsonEntity.Emoji.Name;

    public bool Animated => _jsonEntity.Emoji.Animated;

    public bool IsStandard => Id == null;

    internal MessageReaction(JsonModels.JsonMessageReaction jsonEntity, BotClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }
}
