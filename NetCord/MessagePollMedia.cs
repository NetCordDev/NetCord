using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessagePollMedia(JsonMessagePollMedia jsonModel) : IJsonModel<JsonMessagePollMedia>
{
    JsonMessagePollMedia IJsonModel<JsonMessagePollMedia>.JsonModel => jsonModel;
    public string? Text => jsonModel.Text;
    public Emoji? Emoji { get; }

    public MessagePollMedia(JsonMessagePollMedia jsonModel, ulong guildId, RestClient client) : this(jsonModel)
    {
        var emoji = jsonModel.Emoji;
        if (emoji != null)
            Emoji = Emoji.CreateFromJson(emoji, guildId, client);
    }
}
