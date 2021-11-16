using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.AllowedMentionConverter))]
public class AllowedMentions
{
    /// <summary>
    /// <see langword="@everyone"/> and <see langword="@here"/>
    /// </summary>
    public bool Everyone { get; set; } = true;
    /// <summary>
    /// <see langword="null"/> for all
    /// </summary>
    public List<DiscordId>? TheOnlyAllowedRoles { get; set; }
    /// <summary>
    /// <see langword="null"/> for all
    /// </summary>
    public List<DiscordId>? TheOnlyAllowedUsers { get; set; }
    public bool ReplyMention { get; set; } = true;

    public static AllowedMentions All => new();
    public static AllowedMentions None => new()
    {
        TheOnlyAllowedRoles = new(0),
        TheOnlyAllowedUsers = new(0),
        Everyone = false,
        ReplyMention = false,
    };
}