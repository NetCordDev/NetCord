using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class ButtonInteraction : MessageComponentInteraction
{
    public ButtonInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!);
    }

    public override ButtonInteractionData Data { get; }
}
