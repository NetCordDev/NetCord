using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessageSnapshot(JsonMessageSnapshot jsonModel, ulong? guildId, RestClient client) : IJsonModel<JsonMessageSnapshot>
{
    JsonMessageSnapshot IJsonModel<JsonMessageSnapshot>.JsonModel => jsonModel;

    public MessageSnapshotMessage Message { get; } = new(jsonModel.Message, guildId, client);
}
