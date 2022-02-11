using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(AllowedMentionConverter))]
public class AllowedMentionsProperties
{
    /// <summary>
    /// <see langword="@everyone"/> and <see langword="@here"/>
    /// </summary>
    public bool Everyone { get; set; } = true;
    /// <summary>
    /// <see langword="null"/> for all
    /// </summary>
    public IEnumerable<DiscordId>? TheOnlyAllowedRoles { get; set; }
    /// <summary>
    /// <see langword="null"/> for all
    /// </summary>
    public IEnumerable<DiscordId>? TheOnlyAllowedUsers { get; set; }
    public bool ReplyMention { get; set; }

    public static AllowedMentionsProperties All => new()
    {
        ReplyMention = true
    };

    public static AllowedMentionsProperties None => new()
    {
        TheOnlyAllowedRoles = Enumerable.Empty<DiscordId>(),
        TheOnlyAllowedUsers = Enumerable.Empty<DiscordId>(),
        Everyone = false,
    };

    private class AllowedMentionConverter : JsonConverter<AllowedMentionsProperties>
    {
        public override AllowedMentionsProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, AllowedMentionsProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            List<string> list = new();
            if (value.TheOnlyAllowedRoles == null)
                list.Add("roles");
            else
            {
                writer.WritePropertyName("roles");
                JsonSerializer.Serialize(writer, value.TheOnlyAllowedRoles);
            }
            if (value.TheOnlyAllowedUsers == null)
                list.Add("users");
            else
            {
                writer.WritePropertyName("users");
                JsonSerializer.Serialize(writer, value.TheOnlyAllowedUsers);
            }
            if (value.Everyone)
                list.Add("everyone");
            writer.WritePropertyName("parse");
            JsonSerializer.Serialize(writer, list);

            if (value.ReplyMention == true)
            {
                writer.WritePropertyName("replied_user");
                JsonSerializer.Serialize(writer, value.ReplyMention);
            }
            writer.WriteEndObject();
        }
    }
}