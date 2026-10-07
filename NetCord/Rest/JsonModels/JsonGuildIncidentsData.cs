using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGuildIncidentsData
{
    [JsonPropertyName("invites_disabled_until")]
    public DateTimeOffset? InvitesDisabledUntil { get; set; }

    [JsonPropertyName("dms_disabled_until")]
    public DateTimeOffset? DmsDisabledUntil { get; set; }

    [JsonPropertyName("dm_spam_detected_at")]
    public DateTimeOffset? DmSpamDetectedAt { get; set; }

    [JsonPropertyName("raid_detected_at")]
    public DateTimeOffset? RaidDetectedAt { get; set; }
}

