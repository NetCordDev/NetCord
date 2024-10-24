using NetCord.JsonModels;

namespace NetCord;

public class MessagePollMedia : IJsonModel<JsonMessagePollMedia>
{
    JsonMessagePollMedia IJsonModel<JsonMessagePollMedia>.JsonModel => _jsonModel;
    private readonly JsonMessagePollMedia _jsonModel;

    public string? Text => _jsonModel.Text;
    public EmojiReference? Emoji { get; }

    public MessagePollMedia(JsonMessagePollMedia jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
