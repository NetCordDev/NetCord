namespace NetCord;

public class ApplicationCommandPermission : Entity
{
    private readonly JsonModels.JsonApplicationCommandPermission _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public ApplicationCommandPermissionType Type => _jsonEntity.Type;

    public bool Permission => _jsonEntity.Permission;

    internal ApplicationCommandPermission(JsonModels.JsonApplicationCommandPermission jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}