using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class UsernamesGuildUsersSearchQuery : IGuildUsersSearchQuery
{
    private static readonly JsonEncodedText _usernames = JsonEncodedText.Encode("usernames");

    public UsernamesGuildUsersSearchQuery(IEnumerable<string> usernames)
    {
        Usernames = usernames;
    }

    [JsonPropertyName("or_query")]
    public IEnumerable<string> Usernames { get; set; }

    public void Serialize(Utf8JsonWriter writer)
    {
        writer.WritePropertyName(_usernames);
        JsonSerializer.Serialize(writer, this, Serialization.Default.UsernamesGuildUsersSearchQuery);
    }
}
