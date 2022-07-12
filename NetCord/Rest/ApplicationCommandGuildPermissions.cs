namespace NetCord.Rest;

public class ApplicationCommandGuildPermissions : IJsonModel<JsonModels.JsonApplicationCommandGuildPermissions>
{
    JsonModels.JsonApplicationCommandGuildPermissions IJsonModel<JsonModels.JsonApplicationCommandGuildPermissions>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandGuildPermissions _jsonModel;

    public Snowflake CommandId => _jsonModel.CommandId;

    public Snowflake ApplicationId => _jsonModel.ApplicationId;

    public Snowflake GuildId => _jsonModel.GuildId;

    public IReadOnlyDictionary<Snowflake, ApplicationCommandPermission> Permissions { get; }

    public ApplicationCommandGuildPermissions(JsonModels.JsonApplicationCommandGuildPermissions jsonModel)
    {
        _jsonModel = jsonModel;
        Permissions = jsonModel.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandPermission(p));
    }
}