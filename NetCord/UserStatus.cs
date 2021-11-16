using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserStatus
    {
        Idle,
        Dnd,
        Online,
        Offline,
    }
}