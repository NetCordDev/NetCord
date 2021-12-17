namespace NetCord;

public class MessageReaction : ClientEntity
{
    private readonly JsonModels.JsonMessageReaction _jsonEntity;

    public int Count => _jsonEntity.Count;

    public bool Me => _jsonEntity.Me;

#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    public override DiscordId? Id => _jsonEntity.Emoji.Id;
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

    public string? Name => _jsonEntity.Emoji.Name;

    public bool Animated => _jsonEntity.Emoji.Animated;

    public bool IsStandard => Id == null;

    internal MessageReaction(JsonModels.JsonMessageReaction jsonEntity, BotClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }
}
