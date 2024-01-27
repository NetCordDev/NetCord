using System.Text.Json;

namespace NetCord.Rest;

public interface IGuildUsersSearchQuery
{
    public void Serialize(Utf8JsonWriter writer);
}
