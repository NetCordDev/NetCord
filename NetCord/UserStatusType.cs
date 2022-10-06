using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<UserStatusType>))]
public enum UserStatusType
{
    Online,
    Dnd,
    Idle,
    Invisible,
    Offline,
}
