using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonGuildScheduledEventRecurrenceRuleNWeekday
{
    [JsonPropertyName("n")]
    public int N { get; set; }

    [JsonPropertyName("day")]
    public GuildScheduledEventRecurrenceRuleWeekday Day { get; set; }
}
