using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAutoModerationRuleTriggerMetadata
{
    [JsonPropertyName("keyword_filter")]
    public string[]? KeywordFilter { get; set; }

    [JsonPropertyName("regex_patterns")]
    public string[] RegexPatterns { get; set; }

    [JsonPropertyName("presets")]
    public AutoModerationRuleKeywordPresetType[]? Presets { get; set; }

    [JsonPropertyName("allow_list")]
    public string[]? AllowList { get; set; }

    [JsonPropertyName("mention_total_limit")]
    public int? MentionTotalLimit { get; set; }

    [JsonPropertyName("mention_raid_protection_enabled")]
    public bool MentionRaidProtectionEnabled { get; set; }
}
