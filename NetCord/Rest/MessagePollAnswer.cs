using System.Diagnostics.CodeAnalysis;

using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollAnswer : IJsonModel<JsonMessagePollAnswer>
{
    public JsonMessagePollAnswer JsonModel { get; }
    public ulong AnswerId { get; }
    public MessagePollMedia PollMedia { get; }

    [SetsRequiredMembers]
    public MessagePollAnswer(JsonMessagePollAnswer jsonModel, ulong guildId, RestClient client)
    {
        JsonModel = jsonModel;
        AnswerId = jsonModel.AnswerId;
        PollMedia = new(jsonModel.PollMedia, guildId, client);
    }
}
