using NetCord.Rest;

namespace NetCord.Gateway;

public class RoleEventArgs(JsonModels.EventArgs.JsonRoleEventArgs jsonModel, RestClient client) : IJsonModel<JsonModels.EventArgs.JsonRoleEventArgs>
{
    JsonModels.EventArgs.JsonRoleEventArgs IJsonModel<JsonModels.EventArgs.JsonRoleEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public Role Role { get; } = new(jsonModel.Role, jsonModel.GuildId, client);
}
