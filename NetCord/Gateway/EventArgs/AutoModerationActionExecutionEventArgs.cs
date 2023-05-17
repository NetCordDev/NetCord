namespace NetCord.Gateway;

public class AutoModerationActionExecutionEventArgs : IJsonModel<JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs>
{
    JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs IJsonModel<JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs _jsonModel;

    public AutoModerationActionExecutionEventArgs(JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
        Action = new(_jsonModel.Action);
    }

    public ulong GuildId => _jsonModel.GuildId;

    public AutoModerationAction Action { get; }

    public ulong RuleId => _jsonModel.RuleId;

    public AutoModerationRuleTriggerType RuleTriggerType => _jsonModel.RuleTriggerType;

    public ulong UserId => _jsonModel.UserId;

    public ulong? ChannelId => _jsonModel.ChannelId;

    public ulong? MessageId => _jsonModel.MessageId;

    public ulong? AlertSystemMessageId => _jsonModel.AlertSystemMessageId;

    public string Content => _jsonModel.Content;

    public string? MatchedKeyword => _jsonModel.MatchedKeyword;

    public string? MatchedContent => _jsonModel.MatchedContent;
}
