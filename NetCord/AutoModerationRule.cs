using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationRule : Entity, IJsonModel<JsonAutoModerationRule>
{
    JsonAutoModerationRule IJsonModel<JsonAutoModerationRule>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationRule _jsonModel;

    public AutoModerationRule(JsonAutoModerationRule jsonModel)
    {
        _jsonModel = jsonModel;
        TriggerMetadata = new(_jsonModel.TriggerMetadata);
        Actions = _jsonModel.Actions.Select(a => new AutoModerationAction(a));
    }

    public override Snowflake Id => _jsonModel.Id;

    public Snowflake GuildId => _jsonModel.GuildId;

    public string Name => _jsonModel.Name;

    public Snowflake CreatorId => _jsonModel.CreatorId;

    public AutoModerationRuleEventType EventType => _jsonModel.EventType;

    public AutoModerationRuleTriggerType TriggerType => _jsonModel.TriggerType;

    public AutoModerationRuleTriggerMetadata TriggerMetadata { get; }

    public IEnumerable<AutoModerationAction> Actions { get; }

    public bool Enabled => _jsonModel.Enabled;

    public IReadOnlyCollection<Snowflake> ExemptRoles => _jsonModel.ExemptRoles;

    public IReadOnlyCollection<Snowflake> ExemptChannels => _jsonModel.ExemptChannels;
}