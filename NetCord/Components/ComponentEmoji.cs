namespace NetCord;

public class ComponentEmoji : Entity
{
    private readonly JsonModels.JsonEmoji _jsonEntity;

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

    public string Name => _jsonEntity.Name!;

    public bool Animated => _jsonEntity.Animated;

    internal ComponentEmoji(JsonModels.JsonEmoji jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}