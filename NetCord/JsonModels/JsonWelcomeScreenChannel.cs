﻿using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonWelcomeScreenChannel
{
    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("emoji_id")]
    public DiscordId? EmojiId { get; init; }

    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; init; }
}