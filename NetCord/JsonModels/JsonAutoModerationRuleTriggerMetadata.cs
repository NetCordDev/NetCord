using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAutoModerationRuleTriggerMetadata
{
    [JsonPropertyName("keyword_filter")]
    public string[]? KeywordFilter { get; init; }

    [JsonPropertyName("presets")]
    public AutoModerationRuleKeywordPresetType[]? Presets { get; init; }

    [JsonPropertyName("allow_list")]
    public string[]? AllowList { get; init; }

    [JsonPropertyName("mention_total_limit")]
    public int? MentionTotalLimit { get; init; }
}
