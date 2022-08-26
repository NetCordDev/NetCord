using NetCord.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class AutoModerationActionExecutionEventArgs : IJsonModel<JsonAutoModerationActionExecutionEventArgs>
{
    JsonAutoModerationActionExecutionEventArgs IJsonModel<JsonAutoModerationActionExecutionEventArgs>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationActionExecutionEventArgs _jsonModel;

    public AutoModerationActionExecutionEventArgs(JsonAutoModerationActionExecutionEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
        Action = new(_jsonModel.Action);
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public AutoModerationAction Action { get; }

    public Snowflake RuleId => _jsonModel.RuleId;

    public AutoModerationRuleTriggerType RuleTriggerType => _jsonModel.RuleTriggerType;

    public Snowflake UserId => _jsonModel.UserId;

    public Snowflake? ChannelId => _jsonModel.ChannelId;

    public Snowflake? MessageId => _jsonModel.MessageId;

    public Snowflake? AlertSystemMessageId => _jsonModel.AlertSystemMessageId;

    public string Content => _jsonModel.Content;

    public string? MatchedKeyword => _jsonModel.MatchedKeyword;

    public string? MatchedContent => _jsonModel.MatchedContent;
}
