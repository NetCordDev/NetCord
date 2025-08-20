using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class CurrentGuildUserOptions
{
    internal CurrentGuildUserOptions()
    {
    }

    /// <summary>
    /// New nickname, empty to remove nickname.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }
}
