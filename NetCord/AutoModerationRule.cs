using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class AutoModerationRule(JsonAutoModerationRule jsonModel, RestClient client) : ClientEntity(jsonModel, client)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public string Name { get; } = jsonModel.Name;

    public ulong CreatorId { get; } = jsonModel.CreatorId;

    public AutoModerationRuleEventType EventType { get; } = jsonModel.EventType;

    public AutoModerationRuleTriggerType TriggerType { get; } = jsonModel.TriggerType;

    public AutoModerationRuleTriggerMetadata TriggerMetadata { get; } = new(jsonModel.TriggerMetadata);

    public IReadOnlyList<AutoModerationAction> Actions { get; } = [.. jsonModel.Actions.Select(a => new AutoModerationAction(a))];

    public bool Enabled { get; } = jsonModel.Enabled;

    public IReadOnlyList<ulong> ExemptRoles { get; } = jsonModel.ExemptRoles;

    public IReadOnlyList<ulong> ExemptChannels { get; } = jsonModel.ExemptChannels;
}
