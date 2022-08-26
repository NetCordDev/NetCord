using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling))]
public enum UserStatusType
{
    Online,
    Dnd,
    Idle,
    Invisible,
    Offline,
}
