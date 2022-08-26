namespace NetCord;

public class PermissionOverwrite : Entity, IJsonModel<JsonModels.JsonPermissionOverwrite>
{
    JsonModels.JsonPermissionOverwrite IJsonModel<JsonModels.JsonPermissionOverwrite>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonPermissionOverwrite _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public PermissionOverwriteType Type => _jsonModel.Type;

    public Permission Allowed { get; }

    public Permission Denied { get; }

    public PermissionOverwrite(JsonModels.JsonPermissionOverwrite jsonModel)
    {
        _jsonModel = jsonModel;
        Allowed = (Permission)ulong.Parse(jsonModel.Allowed);
        Denied = (Permission)ulong.Parse(jsonModel.Denied);
    }
}
