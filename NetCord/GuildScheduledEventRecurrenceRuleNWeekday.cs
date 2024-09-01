using NetCord.JsonModels;

namespace NetCord;

public class GuildScheduledEventRecurrenceRuleNWeekday(JsonGuildScheduledEventRecurrenceRuleNWeekday jsonModel) : IJsonModel<JsonGuildScheduledEventRecurrenceRuleNWeekday>
{
    JsonGuildScheduledEventRecurrenceRuleNWeekday IJsonModel<JsonGuildScheduledEventRecurrenceRuleNWeekday>.JsonModel => jsonModel;

    public int N => jsonModel.N;

    public GuildScheduledEventRecurrenceRuleWeekday Day => jsonModel.Day;
}
