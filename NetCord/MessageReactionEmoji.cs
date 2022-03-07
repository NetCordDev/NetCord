namespace NetCord;

public class MessageReactionEmoji : Entity
{
    private readonly JsonModels.JsonEmoji _jsonEntity;

    internal MessageReactionEmoji(JsonModels.JsonEmoji jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public override DiscordId Id
    {
        get
        {
            if (IsStandard)
                throw new InvalidOperationException("This emoji has no id");
            return _jsonEntity.Id.GetValueOrDefault();
        }
    }

    public bool IsStandard => !_jsonEntity.Id.HasValue;

    public string? Name => _jsonEntity.Name;

    public bool Animated => _jsonEntity.Animated;
}
