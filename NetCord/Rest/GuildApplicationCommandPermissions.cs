namespace NetCord;

public class GuildApplicationCommandPermissions
{
    private readonly JsonModels.JsonGuildApplicationCommandPermissions _jsonEntity;

    public Snowflake CommandId => _jsonEntity.CommandId;

    public Snowflake ApplicationId => _jsonEntity.ApplicationId;

    public Snowflake GuildId => _jsonEntity.GuildId;

    public IReadOnlyDictionary<Snowflake, ApplicationCommandPermission> Permissions { get; }

    internal GuildApplicationCommandPermissions(JsonModels.JsonGuildApplicationCommandPermissions jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Permissions = jsonEntity.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandPermission(p));
    }
}