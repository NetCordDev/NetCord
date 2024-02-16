namespace NetCord;

public class ApplicationCommandPermission(JsonModels.JsonApplicationCommandGuildPermission jsonModel) : Entity, IJsonModel<JsonModels.JsonApplicationCommandGuildPermission>
{
    JsonModels.JsonApplicationCommandGuildPermission IJsonModel<JsonModels.JsonApplicationCommandGuildPermission>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    public ApplicationCommandGuildPermissionType Type => jsonModel.Type;

    public bool Permission => jsonModel.Permission;
}
