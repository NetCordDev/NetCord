using NetCord.JsonModels;

namespace NetCord;

public class GuildScheduledEventRecurrenceRule(JsonGuildScheduledEventRecurrenceRule jsonModel)
{
    public DateTimeOffset? StartAt { get; } = jsonModel.StartAt;

    public DateTimeOffset? EndAt { get; } = jsonModel.EndAt;

    public GuildScheduledEventRecurrenceRuleFrequency Frequency { get; } = jsonModel.Frequency;

    public int Interval { get; } = jsonModel.Interval;

    public IReadOnlyList<GuildScheduledEventRecurrenceRuleWeekday>? ByWeekday { get; } = jsonModel.ByWeekday;

    public IReadOnlyList<GuildScheduledEventRecurrenceRuleNWeekday>? ByNWeekday { get; } = jsonModel.ByNWeekday?.Select(b => new GuildScheduledEventRecurrenceRuleNWeekday(b)).ToArray();

    public IReadOnlyList<GuildScheduledEventRecurrenceRuleMonth>? ByMonth { get; } = jsonModel.ByMonth;

    public IReadOnlyList<int>? ByMonthDay { get; } = jsonModel.ByMonthDay;

    public IReadOnlyList<int>? ByYearDay { get; } = jsonModel.ByYearDay;

    public int? Count { get; } = jsonModel.Count;
}
