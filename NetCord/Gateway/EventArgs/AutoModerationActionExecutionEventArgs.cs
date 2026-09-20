namespace NetCord.Gateway;

/// <summary>
/// Represents the event arguments for an auto moderation action execution event.
/// </summary>
public class AutoModerationActionExecutionEventArgs(JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs jsonModel) 
    : IJsonModel<JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs>
{
    JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs IJsonModel<JsonModels.EventArgs.JsonAutoModerationActionExecutionEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the guild where the auto moderation action was executed.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The specific action that was executed by the auto moderation system.
    /// </summary>
    public AutoModerationAction Action { get; } = new(jsonModel.Action);

    /// <summary>
    /// The ID of the rule that was triggered.
    /// </summary>
    public ulong RuleId => jsonModel.RuleId;

    /// <summary>
    /// The trigger type of the auto moderation rule.
    /// </summary>
    public AutoModerationRuleTriggerType RuleTriggerType => jsonModel.RuleTriggerType;

    /// <summary>
    /// The ID of the user who triggered the auto moderation rule.
    /// </summary>
    public ulong UserId => jsonModel.UserId;

    /// <summary>
    /// The ID of the channel where the rule was triggered, if applicable.
    /// </summary>
    public ulong? ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// The ID of the message that triggered the rule, if applicable.
    /// </summary>
    public ulong? MessageId => jsonModel.MessageId;

    /// <summary>
    /// The ID of the system alert message created by this execution, if applicable.
    /// </summary>
    public ulong? AlertSystemMessageId => jsonModel.AlertSystemMessageId;

    /// <summary>
    /// The user-generated text content that triggered the rule.
    /// </summary>
    public string Content => jsonModel.Content;

    /// <summary>
    /// The specific keyword that was matched from the rule configuration, if applicable.
    /// </summary>
    public string? MatchedKeyword => jsonModel.MatchedKeyword;

    /// <summary>
    /// The specific substring of text content that matched the rule, if applicable.
    /// </summary>
    public string? MatchedContent => jsonModel.MatchedContent;
}
