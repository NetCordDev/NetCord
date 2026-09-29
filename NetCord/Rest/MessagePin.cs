using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a message pin in a channel, containing the pinned message and the time it was pinned.
/// </summary>
public class MessagePin(JsonMessagePin jsonModel, RestClient client)
{
    /// <summary>
    /// The time the message was pinned.
    /// </summary>
    public DateTimeOffset PinnedAt { get; } = jsonModel.PinnedAt;

    /// <summary>
    /// The pinned message.
    /// </summary>
    public RestMessage Message { get; } = new(jsonModel.Message, client);
}
