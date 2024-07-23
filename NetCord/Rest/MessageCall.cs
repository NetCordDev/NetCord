using NetCord.JsonModels;

namespace NetCord.Rest;

public class MessageCall(JsonMessageCall jsonModel) : IJsonModel<JsonMessageCall>
{
    JsonMessageCall IJsonModel<JsonMessageCall>.JsonModel => jsonModel;

    public IReadOnlyList<ulong> Participants => jsonModel.Participants;

    public DateTimeOffset? EndedAt => jsonModel.EndedAt;
}
