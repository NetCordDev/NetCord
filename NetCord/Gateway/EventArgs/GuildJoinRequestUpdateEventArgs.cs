using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildJoinRequestUpdateEventArgs(JsonModels.EventArgs.JsonGuildJoinRequestUpdateEventArgs jsonModel, RestClient client) : IJsonModel<JsonModels.EventArgs.JsonGuildJoinRequestUpdateEventArgs>
{
    JsonModels.EventArgs.JsonGuildJoinRequestUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildJoinRequestUpdateEventArgs>.JsonModel => jsonModel;

    public GuildJoinRequestStatus Status => jsonModel.Status;

    public GuildJoinRequest Request { get; } = new(jsonModel.Request, client);

    public ulong GuildId => jsonModel.GuildId;
}
