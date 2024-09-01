using NetCord.JsonModels;

namespace NetCord;

public class GuildScheduledEventRecurrenceRule(JsonGuildScheduledEventRecurrenceRule jsonModel) : IJsonModel<JsonGuildScheduledEventRecurrenceRule>
{
    JsonGuildScheduledEventRecurrenceRule IJsonModel<JsonGuildScheduledEventRecurrenceRule>.JsonModel => jsonModel;

    public DateTimeOffset? StartAt => jsonModel.StartAt;

    public DateTimeOffset? EndAt => jsonModel.EndAt;

    public GuildScheduledEventRecurrenceRuleFrequency Frequency => jsonModel.Frequency;

    public int Interval => jsonModel.Interval;

    public GuildScheduledEventRecurrenceRuleWeekday? ByWeekday => jsonModel.ByWeekday;

    public GuildScheduledEventRecurrenceRuleNWeekday ByNWeekday { get; } = new(jsonModel.ByNWeekday);

    public GuildScheduledEventRecurrenceRuleMonth? ByMonth => jsonModel.ByMonth;

    public IReadOnlyList<int>? ByMonthDay => jsonModel.ByMonthDay;

    public IReadOnlyList<int>? ByYearDay => jsonModel.ByYearDay;

    public int? Count => jsonModel.Count;
}
