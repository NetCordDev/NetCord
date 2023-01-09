namespace NetCord.Gateway;

public class RoleDeleteEventArgs : IJsonModel<JsonModels.EventArgs.JsonRoleDeleteEventArgs>
{
    JsonModels.EventArgs.JsonRoleDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonRoleDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonRoleDeleteEventArgs _jsonModel;

    public RoleDeleteEventArgs(JsonModels.EventArgs.JsonRoleDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong GuildId => _jsonModel.GuildId;

    public ulong RoleId => _jsonModel.RoleId;
}
