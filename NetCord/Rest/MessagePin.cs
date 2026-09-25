namespace NetCord.Rest;

/// <summary>
/// Represents a message pin in a channel, containing the pinned message and the time it was pinned.
/// </summary>
public class MessagePin(JsonModels.JsonMessagePin jsonModel, RestClient client) : IJsonModel<JsonModels.JsonMessagePin>
{
    JsonModels.JsonMessagePin IJsonModel<JsonModels.JsonMessagePin>.JsonModel => jsonModel;

    /// <summary>
    /// The time the message was pinned.
    /// </summary>
    public DateTimeOffset PinnedAt => jsonModel.PinnedAt;

    /// <summary>
    /// The pinned message.
    /// </summary>
    public RestMessage Message { get; } = new(jsonModel.Message, client);
}
