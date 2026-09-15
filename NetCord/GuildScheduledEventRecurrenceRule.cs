using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a recurrence rule for a guild scheduled event.
/// </summary>
/// <remarks>
/// Currently, Discord enforces several limitations on recurrence rules:
/// <list type="bullet">
///     <item><description>The fields <see cref="Count"/>, <see cref="EndAt"/>, and <see cref="ByYearDay"/> cannot be set by client/application.</description></item>
///     <item><description><see cref="ByWeekday"/> and <see cref="ByNWeekday"/> are mutually exclusive.</description></item>
///     <item><description><see cref="ByMonth"/> and <see cref="ByMonthDay"/> are mutually exclusive with <see cref="ByWeekday"/>.</description></item>
///     <item><description><see cref="ByWeekday"/> is only valid for <see cref="GuildScheduledEventRecurrenceRuleFrequency.Daily"/> and <see cref="GuildScheduledEventRecurrenceRuleFrequency.Weekly"/> events.</description></item>
///     <item><description><see cref="ByNWeekday"/> is only valid for <see cref="GuildScheduledEventRecurrenceRuleFrequency.Monthly"/> events.</description></item>
///     <item><description><see cref="ByMonth"/> and <see cref="ByMonthDay"/> are only valid for <see cref="GuildScheduledEventRecurrenceRuleFrequency.Yearly"/> events and both must be provided together.</description></item>
///     <item><description><see cref="Interval"/> can only be set to a value other than <c>1</c> (specifically <c>2</c>) when <see cref="Frequency"/> is <see cref="GuildScheduledEventRecurrenceRuleFrequency.Weekly"/>.</description></item>
/// </list>
/// </remarks>
public class GuildScheduledEventRecurrenceRule(JsonGuildScheduledEventRecurrenceRule jsonModel) : IJsonModel<JsonGuildScheduledEventRecurrenceRule>
{
    JsonGuildScheduledEventRecurrenceRule IJsonModel<JsonGuildScheduledEventRecurrenceRule>.JsonModel => jsonModel;

    /// <summary>
    /// The starting time of the recurrence rule.
    /// </summary>
    public DateTimeOffset? StartAt => jsonModel.StartAt;

    /// <summary>
    /// The ending time of the recurrence rule.
    /// </summary>
    /// <remarks>
    /// Currently, this field cannot be set by the client or application.
    /// </remarks>
    public DateTimeOffset? EndAt => jsonModel.EndAt;

    /// <summary>
    /// The frequency of the recurrence rule.
    /// </summary>
    public GuildScheduledEventRecurrenceRuleFrequency Frequency => jsonModel.Frequency;

    /// <summary>
    /// The spacing between instances of the event.
    /// </summary>
    /// <remarks>
    /// Can only be set to a value other than <c>1</c> (specifically <c>2</c> for "every-other week" events) when <see cref="Frequency"/> is set to <see cref="GuildScheduledEventRecurrenceRuleFrequency.Weekly"/>.
    /// </remarks>
    public int Interval => jsonModel.Interval;

    /// <summary>
    /// The set of specific weekdays on which the event recurs.
    /// </summary>
    /// <remarks>
    /// Only valid for <see cref="GuildScheduledEventRecurrenceRuleFrequency.Daily"/> and <see cref="GuildScheduledEventRecurrenceRuleFrequency.Weekly"/> events.
    /// <list type="bullet">
    ///     <item><description>For daily events, the set must match a known allowed set of weekdays (e.g., Monday–Friday, Saturday–Sunday).</description></item>
    ///     <item><description>For weekly events, this array currently can only have a length of 1 (a single day per week).</description></item>
    /// </list>
    /// Mutually exclusive with <see cref="ByNWeekday"/> and <see cref="ByMonth"/> / <see cref="ByMonthDay"/>.
    /// </remarks>
    public IReadOnlyList<GuildScheduledEventRecurrenceRuleWeekday>? ByWeekday => jsonModel.ByWeekday;

    /// <summary>
    /// The set of specific nth-weekdays of a month on which the event recurs.
    /// </summary>
    /// <remarks>
    /// Only valid for <see cref="GuildScheduledEventRecurrenceRuleFrequency.Monthly"/> events.
    /// Currently, this array can only have a length of 1.
    /// Mutually exclusive with <see cref="ByWeekday"/>.
    /// </remarks>
    public IReadOnlyList<GuildScheduledEventRecurrenceRuleNWeekday>? ByNWeekday { get; } = jsonModel.ByNWeekday?.Select(b => new GuildScheduledEventRecurrenceRuleNWeekday(b)).ToArray();

    /// <summary>
    /// The set of months in which the event recurs.
    /// </summary>
    /// <remarks>
    /// Only valid for <see cref="GuildScheduledEventRecurrenceRuleFrequency.Yearly"/> events.
    /// Must be provided together with <see cref="ByMonthDay"/>, and both arrays must have a length of 1.
    /// Mutually exclusive with <see cref="ByWeekday"/>.
    /// </remarks>
    public IReadOnlyList<GuildScheduledEventRecurrenceRuleMonth>? ByMonth => jsonModel.ByMonth;

    /// <summary>
    /// The set of days of the month on which the event recurs.
    /// </summary>
    /// <remarks>
    /// Only valid for <see cref="GuildScheduledEventRecurrenceRuleFrequency.Yearly"/> events.
    /// Must be provided together with <see cref="ByMonth"/>, and both arrays must have a length of 1.
    /// Mutually exclusive with <see cref="ByWeekday"/>.
    /// </remarks>
    public IReadOnlyList<int>? ByMonthDay => jsonModel.ByMonthDay;

    /// <summary>
    /// The set of days of the year on which the event recurs.
    /// </summary>
    /// <remarks>
    /// Currently, this field cannot be set by the client or application.
    /// </remarks>
    public IReadOnlyList<int>? ByYearDay => jsonModel.ByYearDay;

    /// <summary>
    /// The total times the event should recur before ending.
    /// </summary>
    /// <remarks>
    /// Currently, this field cannot be set by the client or application.
    /// </remarks>
    public int? Count => jsonModel.Count;
}
