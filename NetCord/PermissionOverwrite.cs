namespace NetCord;

public class PermissionOverwrite : Entity, IJsonModel<JsonModels.JsonPermissionOverwrite>
{
    JsonModels.JsonPermissionOverwrite IJsonModel<JsonModels.JsonPermissionOverwrite>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonPermissionOverwrite _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public PermissionOverwriteType Type => _jsonModel.Type;

    public Permission Allowed => _jsonModel.Allowed;

    public Permission Denied => _jsonModel.Denied;

    public PermissionOverwrite(JsonModels.JsonPermissionOverwrite jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
