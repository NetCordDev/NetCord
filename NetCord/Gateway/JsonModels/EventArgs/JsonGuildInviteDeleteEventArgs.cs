using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public partial class JsonGuildInviteDeleteEventArgs
{
    [JsonPropertyName("channel_id")]
    public ulong InviteChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("code")]
    public string InviteCode { get; set; }

    [JsonSerializable(typeof(JsonGuildInviteDeleteEventArgs))]
    public partial class JsonGuildInviteDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildInviteDeleteEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
