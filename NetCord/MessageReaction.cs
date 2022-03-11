namespace NetCord;

public class MessageReaction
{
    private readonly JsonModels.JsonMessageReaction _jsonEntity;

    public int Count => _jsonEntity.Count;

    public bool Me => _jsonEntity.Me;

    public DiscordId? Id => _jsonEntity.Emoji.Id;

    public string? Name => _jsonEntity.Emoji.Name;

    public bool Animated => _jsonEntity.Emoji.Animated;

    public bool IsStandard => !_jsonEntity.Emoji.Id.HasValue;

    internal MessageReaction(JsonModels.JsonMessageReaction jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
    }
}
