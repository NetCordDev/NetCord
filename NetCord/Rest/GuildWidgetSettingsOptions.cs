using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildWidgetSettingsOptions
{
    internal GuildWidgetSettingsOptions()
    {
    }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonSerializable(typeof(GuildWidgetSettingsOptions))]
    public partial class GuildWidgetSettingsOptionsSerializerContext : JsonSerializerContext
    {
        public static GuildWidgetSettingsOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
