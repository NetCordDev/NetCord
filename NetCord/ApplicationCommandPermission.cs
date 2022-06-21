namespace NetCord;

public class ApplicationCommandPermission : Entity, IJsonModel<JsonModels.JsonApplicationCommandPermission>
{
    JsonModels.JsonApplicationCommandPermission IJsonModel<JsonModels.JsonApplicationCommandPermission>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandPermission _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public ApplicationCommandPermissionType Type => _jsonModel.Type;

    public bool Permission => _jsonModel.Permission;

    public ApplicationCommandPermission(JsonModels.JsonApplicationCommandPermission jsonModel)
    {
        _jsonModel = jsonModel;
    }
}