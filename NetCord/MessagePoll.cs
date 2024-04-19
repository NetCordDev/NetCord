using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessagePoll(JsonMessagePoll jsonModel, ulong guildId, RestClient client) : IJsonModel<JsonMessagePoll>
{
    JsonMessagePoll IJsonModel<JsonMessagePoll>.JsonModel => jsonModel;
    public MessagePollMedia Question = new(jsonModel.Question, guildId, client);
    public IReadOnlyList<MessagePollAnswer> Answers = jsonModel.Answers.Select(x => new MessagePollAnswer(x, guildId, client)).ToArray();
    public bool AllowMultiselect => jsonModel.AllowMultiselect;
    public MessagePollLayoutType LayoutType => jsonModel.LayoutType;
    // Non-expiring posts are possible in the future, see: https://github.com/discord/discord-api-docs/blob/e4bdf50f11f9ca61ace2636285e029a2b3dfd0ec/docs/resources/Poll.md#poll-object
    public DateTimeOffset? ExpiresAt => jsonModel.ExpiresAt;
    public MessagePollResults Results = new(jsonModel.Results);
}
