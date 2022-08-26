namespace NetCord;

public class ApplicationCommandPermission : Entity, IJsonModel<JsonModels.JsonApplicationCommandGuildPermission>
{
    JsonModels.JsonApplicationCommandGuildPermission IJsonModel<JsonModels.JsonApplicationCommandGuildPermission>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandGuildPermission _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public ApplicationCommandGuildPermissionType Type => _jsonModel.Type;

    public bool Permission => _jsonModel.Permission;

    public ApplicationCommandPermission(JsonModels.JsonApplicationCommandGuildPermission jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
