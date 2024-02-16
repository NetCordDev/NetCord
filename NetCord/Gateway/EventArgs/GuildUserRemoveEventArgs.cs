using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildUserRemoveEventArgs(JsonModels.EventArgs.JsonGuildUserRemoveEventArgs jsonModel, RestClient client) : IJsonModel<JsonModels.EventArgs.JsonGuildUserRemoveEventArgs>
{
    JsonModels.EventArgs.JsonGuildUserRemoveEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildUserRemoveEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public User User { get; } = new(jsonModel.User, client);
}
