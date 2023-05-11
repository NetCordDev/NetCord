using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationRuleTriggerMetadata : IJsonModel<JsonAutoModerationRuleTriggerMetadata>
{
    JsonAutoModerationRuleTriggerMetadata IJsonModel<JsonAutoModerationRuleTriggerMetadata>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationRuleTriggerMetadata _jsonModel;

    public AutoModerationRuleTriggerMetadata(JsonAutoModerationRuleTriggerMetadata jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public IReadOnlyList<string>? KeywordFilter => _jsonModel.KeywordFilter;

    public IReadOnlyList<string>? RegexPatterns => _jsonModel.RegexPatterns;

    public IReadOnlyList<AutoModerationRuleKeywordPresetType>? Presets => _jsonModel.Presets;

    public IReadOnlyList<string>? AllowList => _jsonModel.AllowList;

    public int? MentionTotalLimit => _jsonModel.MentionTotalLimit;

    public bool MentionRaidProtectionEnabled => _jsonModel.MentionRaidProtectionEnabled;
}
