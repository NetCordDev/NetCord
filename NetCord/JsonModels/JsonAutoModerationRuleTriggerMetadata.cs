using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonAutoModerationRuleTriggerMetadata
{
    [JsonPropertyName("keyword_filter")]
    public string[]? KeywordFilter { get; set; }

    [JsonPropertyName("presets")]
    public AutoModerationRuleKeywordPresetType[]? Presets { get; set; }

    [JsonPropertyName("allow_list")]
    public string[]? AllowList { get; set; }

    [JsonPropertyName("mention_total_limit")]
    public int? MentionTotalLimit { get; set; }

    [JsonSerializable(typeof(JsonAutoModerationRuleTriggerMetadata))]
    public partial class JsonAutoModerationRuleTriggerMetadataSerializerContext : JsonSerializerContext
    {
        public static JsonAutoModerationRuleTriggerMetadataSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
