namespace NetCord;

/// <summary>
/// Indicates how long a thread must be inactive before auto-archiving occurs.
/// </summary>
public enum ThreadArchiveDuration
{
    /// <summary>
    /// Auto-archiving will occur after an hour of inactivity.
    /// </summary>
    OneHour = 60,

    /// <summary>
    /// Auto-archiving will occur after one day of inactivity.
    /// </summary>
    OneDay = 24 * OneHour,

    /// <summary>
    /// Auto-archiving will occur after three days of inactivity.
    /// </summary>
    ThreeDays = 3 * OneDay,

    /// <summary>
    /// Auto-archiving will occur after one week of inactivity.
    /// </summary>
    OneWeek = 7 * OneDay,
}
