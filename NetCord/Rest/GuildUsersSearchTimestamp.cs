using System.Numerics;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

public struct GuildUsersSearchTimestamp(DateTimeOffset guildJoinedAt, ulong userId) : IMinMaxValue<GuildUsersSearchTimestamp>
{
    public static GuildUsersSearchTimestamp MaxValue { get; } = new(DateTimeOffset.MaxValue, long.MaxValue);

    public static GuildUsersSearchTimestamp MinValue { get; } = new(DateTimeOffset.FromUnixTimeMilliseconds(1), 1);

    [JsonConverter(typeof(MillisecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("guild_joined_at")]
    public DateTimeOffset GuildJoinedAt { get; set; } = guildJoinedAt;

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; } = userId;
}
