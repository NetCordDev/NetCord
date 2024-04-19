﻿using NetCord.JsonModels;

namespace NetCord;

public class MessagePollResults(JsonMessagePollResults jsonModel) : IJsonModel<JsonMessagePollResults>
{
    JsonMessagePollResults IJsonModel<JsonMessagePollResults>.JsonModel => jsonModel;
    public bool IsFinalized => jsonModel.IsFinalized;
    public IReadOnlyList<MessagePollAnswerCount> Answers = jsonModel.Answers.Select(x => new MessagePollAnswerCount(x)).ToArray();
    public bool ContainsAnswers => Answers.Count != 0;
}
