using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an auto moderation rule within a guild.
/// </summary>
public partial class AutoModerationRule(JsonAutoModerationRule jsonModel, RestClient client) 
    : ClientEntity(client), IJsonModel<JsonAutoModerationRule>
{
    JsonAutoModerationRule IJsonModel<JsonAutoModerationRule>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the auto moderation rule.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The ID of the guild which the auto moderation rule belongs to.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The name of the rule.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The ID of the user who created the rule.
    /// </summary>
    public ulong CreatorId => jsonModel.CreatorId;

    /// <summary>
    /// The type of event that triggers the rule execution.
    /// </summary>
    public AutoModerationRuleEventType EventType => jsonModel.EventType;

    /// <summary>
    /// The type of trigger that determines what content is checked by the rule.
    /// </summary>
    public AutoModerationRuleTriggerType TriggerType => jsonModel.TriggerType;

    /// <summary>
    /// Additional metadata associated with the rule's trigger.
    /// </summary>
    public AutoModerationRuleTriggerMetadata TriggerMetadata { get; } = new(jsonModel.TriggerMetadata);

    /// <summary>
    /// The actions that will execute when the rule is triggered.
    /// </summary>
    public IReadOnlyList<AutoModerationAction> Actions { get; }
        = jsonModel.Actions.Select(a => new AutoModerationAction(a)).ToArray();

    /// <summary>
    /// A value indicating whether the rule is currently enabled.
    /// </summary>
    public bool Enabled => jsonModel.Enabled;

    /// <summary>
    /// The IDs of roles that are exempt from this rule's evaluation.
    /// </summary>
    public IReadOnlyList<ulong> ExemptRoles => jsonModel.ExemptRoles;

    /// <summary>
    /// The IDs of channels that are exempt from this rule's evaluation.
    /// </summary>
    public IReadOnlyList<ulong> ExemptChannels => jsonModel.ExemptChannels;
}
