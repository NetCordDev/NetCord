namespace NetCord;

public class GuildApplicationCommandPermissions
{
    private readonly JsonModels.JsonGuildApplicationCommandPermissions _jsonEntity;

    public DiscordId CommandId => _jsonEntity.CommandId;

    public DiscordId ApplicationId => _jsonEntity.ApplicationId;

    public DiscordId GuildId => _jsonEntity.GuildId;

    public IReadOnlyDictionary<DiscordId, ApplicationCommandPermission> Permissions { get; }

    internal GuildApplicationCommandPermissions(JsonModels.JsonGuildApplicationCommandPermissions jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Permissions = jsonEntity.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandPermission(p));
    }
}