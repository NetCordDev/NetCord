namespace NetCord;

public class MessageReaction : ClientEntity
{
    private readonly JsonModels.JsonMessageReaction _jsonEntity;

    public int Count => _jsonEntity.Count;

    public bool Me => _jsonEntity.Me;

    public override DiscordId Id
    {
        get
        {
            if (!_jsonEntity.Emoji.Id.HasValue)
                throw new InvalidOperationException("This reaction emoji has no id");
            return _jsonEntity.Emoji.Id.GetValueOrDefault();
        }
    }

    public string? Name => _jsonEntity.Emoji.Name;

    public bool Animated => _jsonEntity.Emoji.Animated;

    public bool IsStandard => !_jsonEntity.Emoji.Id.HasValue;

    internal MessageReaction(JsonModels.JsonMessageReaction jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }
}
