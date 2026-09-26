namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class TypingScopeProperties
{
    /// <summary>
    /// The interval at which the typing state should be triggered. If not specified, it defaults to 7 seconds.
    /// </summary>
    public TimeSpan? Interval { get; set; }

    /// <summary>
    /// The time provider used for triggering the typing state. Defaults to <see cref="TimeProvider.System"/>.
    /// </summary>
    public TimeProvider? TimeProvider { get; set; }
}
