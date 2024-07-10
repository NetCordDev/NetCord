using NetCord.JsonModels;

namespace NetCord;

public class MessagePoll : IJsonModel<JsonMessagePoll>
{
    JsonMessagePoll IJsonModel<JsonMessagePoll>.JsonModel => _jsonModel;
    private readonly JsonMessagePoll _jsonModel;

    public MessagePoll(JsonMessagePoll jsonModel)
    {
        _jsonModel = jsonModel;

        Question = new(jsonModel.Question);
        Answers = jsonModel.Answers.Select(a => new MessagePollAnswer(a)).ToArray();

        var results = jsonModel.Results;
        if (results is not null)
            Results = new(results);
    }

    public MessagePollMedia Question { get; }
    public IReadOnlyList<MessagePollAnswer> Answers { get; }
    public DateTimeOffset? ExpiresAt => _jsonModel.ExpiresAt;
    public bool AllowMultiselect => _jsonModel.AllowMultiselect;
    public MessagePollLayoutType LayoutType => _jsonModel.LayoutType;
    public MessagePollResults? Results { get; }
}
