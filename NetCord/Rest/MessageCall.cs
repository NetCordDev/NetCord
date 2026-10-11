using NetCord.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a call in a private channel.
/// </summary>
/// <param name="jsonModel"></param>
public class MessageCall(JsonMessageCall jsonModel)
{
    /// <summary>
    /// A list of IDs, corresponding to the call's participating users.
    /// </summary>
    public IReadOnlyList<ulong> Participants { get; } = jsonModel.Participants;

    /// <summary>
    /// The timestamp of the call's end.
    /// </summary>
    public DateTimeOffset? EndedAt { get; } = jsonModel.EndedAt;
}
