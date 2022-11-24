using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class AutoModerationRule : ClientEntity, IJsonModel<JsonAutoModerationRule>
{
    JsonAutoModerationRule IJsonModel<JsonAutoModerationRule>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationRule _jsonModel;

    public AutoModerationRule(JsonAutoModerationRule jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        TriggerMetadata = new(_jsonModel.TriggerMetadata);
        Actions = _jsonModel.Actions.Select(a => new AutoModerationAction(a));
    }

    public override ulong Id => _jsonModel.Id;

    public ulong GuildId => _jsonModel.GuildId;

    public string Name => _jsonModel.Name;

    public ulong CreatorId => _jsonModel.CreatorId;

    public AutoModerationRuleEventType EventType => _jsonModel.EventType;

    public AutoModerationRuleTriggerType TriggerType => _jsonModel.TriggerType;

    public AutoModerationRuleTriggerMetadata TriggerMetadata { get; }

    public IEnumerable<AutoModerationAction> Actions { get; }

    public bool Enabled => _jsonModel.Enabled;

    public IReadOnlyList<ulong> ExemptRoles => _jsonModel.ExemptRoles;

    public IReadOnlyList<ulong> ExemptChannels => _jsonModel.ExemptChannels;

    #region AutoModeration
    public Task<AutoModerationRule> ModifyAsync(Action<AutoModerationRuleOptions> action, RequestProperties? properties = null) => _client.ModifyAutoModerationRuleAsync(GuildId, Id, action, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteAutoModerationRuleAsync(GuildId, Id, properties);
    #endregion
}
