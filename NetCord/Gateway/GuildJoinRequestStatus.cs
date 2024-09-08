﻿using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(JsonConverters.SafeStringEnumConverter<GuildJoinRequestStatus>))]
public enum GuildJoinRequestStatus
{
    [JsonPropertyName("STARTED")]
    Started,
    [JsonPropertyName("PENDING")]
    Pending,
    [JsonPropertyName("REJECTED")]
    Rejected,
    [JsonPropertyName("APPROVED")]
    Approved,
}
