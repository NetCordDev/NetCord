namespace NetCord.Rest;

public class GuildApplicationCommandPermissions : IJsonModel<JsonModels.JsonGuildApplicationCommandPermissions>
{
    JsonModels.JsonGuildApplicationCommandPermissions IJsonModel<JsonModels.JsonGuildApplicationCommandPermissions>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildApplicationCommandPermissions _jsonModel;

    public Snowflake CommandId => _jsonModel.CommandId;

    public Snowflake ApplicationId => _jsonModel.ApplicationId;

    public Snowflake GuildId => _jsonModel.GuildId;

    public IReadOnlyDictionary<Snowflake, ApplicationCommandPermission> Permissions { get; }

    public GuildApplicationCommandPermissions(JsonModels.JsonGuildApplicationCommandPermissions jsonModel)
    {
        _jsonModel = jsonModel;
        Permissions = jsonModel.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandPermission(p));
    }
}