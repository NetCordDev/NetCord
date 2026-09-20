using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents the media displayed within a poll.
/// </summary>
public class MessagePollMedia(JsonMessagePollMedia jsonModel) : IJsonModel<JsonMessagePollMedia>
{
    JsonMessagePollMedia IJsonModel<JsonMessagePollMedia>.JsonModel => jsonModel;

    /// <summary>
    /// The text to display in the poll, up to 300 characters.
    /// </summary>
    public string? Text => jsonModel.Text;

    /// <summary>
    /// The emoji to display in the poll. Only supported for answers.
    /// </summary>
    public EmojiReference? Emoji { get; } = jsonModel.Emoji is { } emoji ? new(emoji) : null;
}
