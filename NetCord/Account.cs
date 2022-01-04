namespace NetCord;

public class Account : Entity
{
    private readonly JsonModels.JsonAccount _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    internal Account(JsonModels.JsonAccount jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}