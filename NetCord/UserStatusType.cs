using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.UserStatusTypeConverter))]
public enum UserStatusType
{
    Online,
    Dnd,
    Idle,
    Invisible,
    Offline,
}