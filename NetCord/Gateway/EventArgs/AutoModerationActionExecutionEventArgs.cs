using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class AutoModerationActionExecutionEventArgs(JsonAutoModerationActionExecutionEventArgs jsonModel)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public AutoModerationAction Action { get; } = new(jsonModel.Action);

    public ulong RuleId { get; } = jsonModel.RuleId;

    public AutoModerationRuleTriggerType RuleTriggerType { get; } = jsonModel.RuleTriggerType;

    public ulong UserId { get; } = jsonModel.UserId;

    public ulong? ChannelId { get; } = jsonModel.ChannelId;

    public ulong? MessageId { get; } = jsonModel.MessageId;

    public ulong? AlertSystemMessageId { get; } = jsonModel.AlertSystemMessageId;

    public string? Content { get; } = jsonModel.Content;

    public string? MatchedKeyword { get; } = jsonModel.MatchedKeyword;

    public string? MatchedContent { get; } = jsonModel.MatchedContent;
}
