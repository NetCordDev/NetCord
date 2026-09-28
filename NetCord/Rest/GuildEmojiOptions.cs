using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents options for modifying a guild emoji.
/// </summary>
/// <remarks>
/// Modifying an emoji created by the current user requires either the <c>CREATE_GUILD_EXPRESSIONS</c>
/// or <c>MANAGE_GUILD_EXPRESSIONS</c> permission. Modifying an emoji created by another user requires
/// the <c>MANAGE_GUILD_EXPRESSIONS</c> permission.
/// </remarks>
[GenerateMethodsForProperties]
public partial class GuildEmojiOptions
{
    internal GuildEmojiOptions()
    {
    }

    /// <summary>
    /// The new name of the emoji.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The IDs of the roles allowed to use the emoji.
    /// </summary>
    /// <remarks>
    /// An emoji cannot have both subscription roles and non-subscription roles.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("roles")]
    public IEnumerable<ulong>? AllowedRoles { get; set; }
}
