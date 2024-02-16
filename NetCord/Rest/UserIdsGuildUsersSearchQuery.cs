using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class UserIdsGuildUsersSearchQuery(IEnumerable<ulong> userIds) : IGuildUsersSearchQuery
{
    private static readonly JsonEncodedText _userId = JsonEncodedText.Encode("user_id");

    [JsonPropertyName("or_query")]
    public IEnumerable<ulong> UserIds { get; set; } = userIds;

    public void Serialize(Utf8JsonWriter writer)
    {
        writer.WritePropertyName(_userId);
        JsonSerializer.Serialize(writer, this, Serialization.Default.UserIdsGuildUsersSearchQuery);
    }
}
