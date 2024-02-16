namespace NetCord.Gateway;

public class RoleDeleteEventArgs(JsonModels.EventArgs.JsonRoleDeleteEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonRoleDeleteEventArgs>
{
    JsonModels.EventArgs.JsonRoleDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonRoleDeleteEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public ulong RoleId => jsonModel.RoleId;
}
