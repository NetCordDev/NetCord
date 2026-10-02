using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationRuleTriggerMetadata(JsonAutoModerationRuleTriggerMetadata jsonModel)
{
    public IReadOnlyList<string>? KeywordFilter { get; } = jsonModel.KeywordFilter;

    public IReadOnlyList<string>? RegexPatterns { get; } = jsonModel.RegexPatterns;

    public IReadOnlyList<AutoModerationRuleKeywordPresetType>? Presets { get; } = jsonModel.Presets;

    public IReadOnlyList<string>? AllowList { get; } = jsonModel.AllowList;

    public int? MentionTotalLimit { get; } = jsonModel.MentionTotalLimit;

    public bool? MentionRaidProtectionEnabled { get; } = jsonModel.MentionRaidProtectionEnabled;
}
