using System.Text.Json.Serialization;

namespace NetCord;

public class AutoModerationRuleTriggerMetadataProperties
{
    [JsonPropertyName("keyword_filter")]
    public IEnumerable<string>? KeywordFilter { get; set; }

    [JsonPropertyName("presets")]
    public IEnumerable<AutoModerationRuleKeywordPresetType>? Presets { get; set; }
}
