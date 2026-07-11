using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents the media displayed within a poll.
/// </summary>
public class MessagePollMedia : IJsonModel<JsonMessagePollMedia>
{
    JsonMessagePollMedia IJsonModel<JsonMessagePollMedia>.JsonModel => _jsonModel;
    private readonly JsonMessagePollMedia _jsonModel;

    /// <summary>
    /// The text to display in the poll, up to 300 characters.
    /// </summary>
    public string? Text => _jsonModel.Text;

    /// <summary>
    /// The emoji to display in the poll. Only supported for answers.
    /// </summary>
    public EmojiReference? Emoji { get; }

    public MessagePollMedia(JsonMessagePollMedia jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
