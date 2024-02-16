namespace NetCord;

public class PermissionOverwrite(JsonModels.JsonPermissionOverwrite jsonModel) : Entity, IJsonModel<JsonModels.JsonPermissionOverwrite>
{
    JsonModels.JsonPermissionOverwrite IJsonModel<JsonModels.JsonPermissionOverwrite>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    public PermissionOverwriteType Type => jsonModel.Type;

    public Permissions Allowed => jsonModel.Allowed;

    public Permissions Denied => jsonModel.Denied;
}
