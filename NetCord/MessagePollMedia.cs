using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessagePollMedia : IJsonModel<JsonMessagePollMedia>
{
    public JsonMessagePollMedia JsonModel { get; }
    
    public string? Text { get; }
    
    public Emoji? Emoji { get; }

    public MessagePollMedia(JsonMessagePollMedia jsonModel, ulong guildId, RestClient client)
    {
        JsonModel = jsonModel;
        Text = jsonModel.Text;

        var emoji = jsonModel.Emoji;
        if (emoji != null)
            Emoji = Emoji.CreateFromJson(emoji, guildId, client);
    }
}
