﻿using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonInteractionResolvedData
{
    [JsonPropertyName("users")]
    public IReadOnlyDictionary<ulong, JsonUser>? Users { get; set; }

    [JsonPropertyName("members")]
    public IReadOnlyDictionary<ulong, JsonGuildUser>? GuildUsers { get; set; }

    [JsonPropertyName("roles")]
    public IReadOnlyDictionary<ulong, JsonRole>? Roles { get; set; }

    [JsonPropertyName("channels")]
    public IReadOnlyDictionary<ulong, JsonChannel>? Channels { get; set; }

    [JsonPropertyName("messages")]
    public IReadOnlyDictionary<ulong, JsonMessage>? Messages { get; set; }

    [JsonPropertyName("attachments")]
    public IReadOnlyDictionary<ulong, JsonAttachment>? Attachments { get; set; }
}
