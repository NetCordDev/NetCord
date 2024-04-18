using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class MessagePoll : IJsonModel<JsonMessagePoll>
{
    public JsonMessagePoll JsonModel { get; }
    public MessagePollMedia Question { get; set; }
    public MessagePollAnswer[] Answers { get; }
    public bool AllowMultiselect { get; set; }
    public MessagePollLayoutType LayoutType { get; set; }
    // Non-expiring posts are possible in the future, see: https://github.com/discord/discord-api-docs/blob/e4bdf50f11f9ca61ace2636285e029a2b3dfd0ec/docs/resources/Poll.md#poll-object
    public DateTimeOffset? ExpireAt { get; }
    public MessagePollResults Results { get; }
    
    public MessagePoll(JsonMessagePoll jsonModel, ulong guildId, RestClient client)
    {
        JsonModel = jsonModel;
        Question = new(jsonModel.Question, guildId, client);
        Answers = jsonModel.Answers.Select(x => new MessagePollAnswer(x, guildId, client)).ToArray();
        AllowMultiselect = jsonModel.AllowMultiselect;
        LayoutType = jsonModel.LayoutType;
        ExpireAt = jsonModel.ExpireAt;
        Results = new(jsonModel.Results);
    }
}
