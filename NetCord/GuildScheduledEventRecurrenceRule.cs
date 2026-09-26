using NetCord.JsonModels;

namespace NetCord;

public class GuildScheduledEventRecurrenceRule : IJsonModel<JsonGuildScheduledEventRecurrenceRule>
{
    JsonGuildScheduledEventRecurrenceRule IJsonModel<JsonGuildScheduledEventRecurrenceRule>.JsonModel => _jsonModel;
    private readonly JsonGuildScheduledEventRecurrenceRule _jsonModel;

    public GuildScheduledEventRecurrenceRule(JsonGuildScheduledEventRecurrenceRule jsonModel)
    {
        _jsonModel = jsonModel;

        if (jsonModel.ByNWeekday is { } byNWeekday)
            ByNWeekday = byNWeekday.Select(b => new GuildScheduledEventRecurrenceRuleNWeekday(b)).ToArray();
    }

    public DateTimeOffset? StartAt => _jsonModel.StartAt;

    public DateTimeOffset? EndAt => _jsonModel.EndAt;

    public GuildScheduledEventRecurrenceRuleFrequency Frequency => _jsonModel.Frequency;

    public int Interval => _jsonModel.Interval;

    public IReadOnlyList<GuildScheduledEventRecurrenceRuleWeekday>? ByWeekday => _jsonModel.ByWeekday;

    public IReadOnlyList<GuildScheduledEventRecurrenceRuleNWeekday>? ByNWeekday { get; }

    public IReadOnlyList<GuildScheduledEventRecurrenceRuleMonth>? ByMonth => _jsonModel.ByMonth;

    public IReadOnlyList<int>? ByMonthDay => _jsonModel.ByMonthDay;

    public IReadOnlyList<int>? ByYearDay => _jsonModel.ByYearDay;

    public int? Count => _jsonModel.Count;
}
