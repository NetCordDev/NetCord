using NetCord.JsonModels;

namespace NetCord;

public class GuildScheduledEventRecurrenceRuleNWeekday(JsonGuildScheduledEventRecurrenceRuleNWeekday jsonModel)
{
    public int N { get; } = jsonModel.N;

    public GuildScheduledEventRecurrenceRuleWeekday Day { get; } = jsonModel.Day;
}
