using NetCord.JsonModels;

namespace NetCord.Rest;

public class AutoModerationRuleTriggerMetadata : IJsonModel<JsonAutoModerationRuleTriggerMetadata>
{
    JsonAutoModerationRuleTriggerMetadata IJsonModel<JsonAutoModerationRuleTriggerMetadata>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationRuleTriggerMetadata _jsonModel;

    public AutoModerationRuleTriggerMetadata(JsonAutoModerationRuleTriggerMetadata jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public IReadOnlyCollection<string>? KeywordFilter => _jsonModel.KeywordFilter;

    public IReadOnlyCollection<AutoModerationRuleKeywordPresetType>? Presets => _jsonModel.Presets;
}