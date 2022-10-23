namespace NetCord.Rest;

public class ApplicationCommandGuildPermissions : IJsonModel<JsonModels.JsonApplicationCommandGuildPermissions>
{
    JsonModels.JsonApplicationCommandGuildPermissions IJsonModel<JsonModels.JsonApplicationCommandGuildPermissions>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandGuildPermissions _jsonModel;

    public ulong CommandId => _jsonModel.CommandId;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public ulong GuildId => _jsonModel.GuildId;

    public IReadOnlyDictionary<ulong, ApplicationCommandPermission> Permissions { get; }

    public ApplicationCommandGuildPermissions(JsonModels.JsonApplicationCommandGuildPermissions jsonModel)
    {
        _jsonModel = jsonModel;
        Permissions = jsonModel.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandPermission(p));
    }
}
