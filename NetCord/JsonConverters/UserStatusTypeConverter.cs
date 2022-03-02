using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

internal class UserStatusTypeConverter : JsonConverter<UserStatusType>
{
    public override UserStatusType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()! switch
        {
            "online" => UserStatusType.Online,
            "dnd" => UserStatusType.Dnd,
            "idle" => UserStatusType.Idle,
            "invisible" => UserStatusType.Invisible,
            "offline" => UserStatusType.Offline,
            _ => (UserStatusType)(-1),
        };
    }

    public override void Write(Utf8JsonWriter writer, UserStatusType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            UserStatusType.Online => "online",
            UserStatusType.Dnd => "dnd",
            UserStatusType.Idle => "idle",
            UserStatusType.Invisible => "invisible",
            UserStatusType.Offline => "offline",
            _ => throw new InvalidDataException($"'{value}' is not valid {nameof(UserStatusType)}"),
        });
    }
}