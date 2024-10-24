﻿using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class AutoModerationRule : ClientEntity, IJsonModel<JsonAutoModerationRule>
{
    public AutoModerationRule(JsonAutoModerationRule jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        TriggerMetadata = new(_jsonModel.TriggerMetadata);
        Actions = _jsonModel.Actions.Select(a => new AutoModerationAction(a)).ToArray();
    }

    JsonAutoModerationRule IJsonModel<JsonAutoModerationRule>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationRule _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public ulong GuildId => _jsonModel.GuildId;

    public string Name => _jsonModel.Name;

    public ulong CreatorId => _jsonModel.CreatorId;

    public AutoModerationRuleEventType EventType => _jsonModel.EventType;

    public AutoModerationRuleTriggerType TriggerType => _jsonModel.TriggerType;

    public AutoModerationRuleTriggerMetadata TriggerMetadata { get; }

    public IReadOnlyList<AutoModerationAction> Actions { get; }

    public bool Enabled => _jsonModel.Enabled;

    public IReadOnlyList<ulong> ExemptRoles => _jsonModel.ExemptRoles;

    public IReadOnlyList<ulong> ExemptChannels => _jsonModel.ExemptChannels;
}
