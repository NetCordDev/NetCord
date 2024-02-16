using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationRuleTriggerMetadata(JsonAutoModerationRuleTriggerMetadata jsonModel) : IJsonModel<JsonAutoModerationRuleTriggerMetadata>
{
    JsonAutoModerationRuleTriggerMetadata IJsonModel<JsonAutoModerationRuleTriggerMetadata>.JsonModel => jsonModel;

    public IReadOnlyList<string>? KeywordFilter => jsonModel.KeywordFilter;

    public IReadOnlyList<string>? RegexPatterns => jsonModel.RegexPatterns;

    public IReadOnlyList<AutoModerationRuleKeywordPresetType>? Presets => jsonModel.Presets;

    public IReadOnlyList<string>? AllowList => jsonModel.AllowList;

    public int? MentionTotalLimit => jsonModel.MentionTotalLimit;

    public bool MentionRaidProtectionEnabled => jsonModel.MentionRaidProtectionEnabled;
}
