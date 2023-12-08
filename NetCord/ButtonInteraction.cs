using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class ButtonInteraction : MessageComponentInteraction
{
    public ButtonInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
        Data = new(jsonModel.Data!);
    }

    public override ButtonInteractionData Data { get; }
}
