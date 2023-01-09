using NetCord.Rest;

namespace NetCord.Gateway;

public class RoleEventArgs : IJsonModel<JsonModels.EventArgs.JsonRoleEventArgs>
{
    JsonModels.EventArgs.JsonRoleEventArgs IJsonModel<JsonModels.EventArgs.JsonRoleEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonRoleEventArgs _jsonModel;

    public RoleEventArgs(JsonModels.EventArgs.JsonRoleEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Role = new(jsonModel.Role, jsonModel.GuildId, client);
    }

    public ulong GuildId => _jsonModel.GuildId;

    public Role Role { get; }
}
