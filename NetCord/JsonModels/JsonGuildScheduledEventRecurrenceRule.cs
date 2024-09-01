using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonGuildScheduledEventRecurrenceRule
{
    [JsonPropertyName("start")]
    public DateTimeOffset? StartAt { get; set; }

    [JsonPropertyName("end")]
    public DateTimeOffset? EndAt { get; set; }

    [JsonPropertyName("frequency")]
    public GuildScheduledEventRecurrenceRuleFrequency Frequency { get; set; }

    [JsonPropertyName("interval")]
    public int Interval { get; set; }

    [JsonPropertyName("by_weekday")]
    public GuildScheduledEventRecurrenceRuleWeekday? ByWeekday { get; set; }

    [JsonPropertyName("by_n_weekday")]
    public JsonGuildScheduledEventRecurrenceRuleNWeekday ByNWeekday { get; set; }

    [JsonPropertyName("by_month")]
    public GuildScheduledEventRecurrenceRuleMonth? ByMonth { get; set; }

    [JsonPropertyName("by_month_day")]
    public int[]? ByMonthDay { get; set; }

    [JsonPropertyName("by_year_day")]
    public int[]? ByYearDay { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; }
}
