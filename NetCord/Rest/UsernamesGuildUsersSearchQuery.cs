using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class UsernamesGuildUsersSearchQuery(IEnumerable<string> usernames) : IGuildUsersSearchQuery
{
    private static readonly JsonEncodedText _usernames = JsonEncodedText.Encode("usernames");

    [JsonPropertyName("or_query")]
    public IEnumerable<string> Usernames { get; set; } = usernames;

    public void Serialize(Utf8JsonWriter writer)
    {
        writer.WritePropertyName(_usernames);
        JsonSerializer.Serialize(writer, this, Serialization.Default.UsernamesGuildUsersSearchQuery);
    }
}
