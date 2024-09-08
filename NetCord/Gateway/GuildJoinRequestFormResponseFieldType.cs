using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(JsonConverters.SafeStringEnumConverter<GuildJoinRequestFormResponseFieldType>))]
public enum GuildJoinRequestFormResponseFieldType
{
    [JsonPropertyName("TERMS")]
    Terms,
    [JsonPropertyName("TEXT_INPUT")]
    TextInput,
    [JsonPropertyName("PARAGRAPH")]
    Paragraph,
    [JsonPropertyName("MULTIPLE_CHOICE")]
    MultipleChoice,
    [JsonPropertyName("VERIFICATION")]
    Verification,
    [JsonPropertyName("FILE_UPLOAD")]
    FileUpload,
}
