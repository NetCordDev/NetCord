using System.Text.Json.Serialization;

namespace NetCord;

public class AutoModerationRuleTriggerMetadataProperties
{
    [JsonPropertyName("keyword_filter")]
    public IEnumerable<string>? KeywordFilter { get; set; }

    [JsonPropertyName("presets")]
    public IEnumerable<AutoModerationRuleKeywordPresetType>? Presets { get; set; }

    [JsonPropertyName("allow_list")]
    public IEnumerable<string>? AllowList { get; set; }

    [JsonPropertyName("mention_total_limit")]
    public int? MentionTotalLimit { get; set; }
}
