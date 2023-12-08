using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class StringMenuInteraction : MessageComponentInteraction
{
    public StringMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
        Data = new(jsonModel.Data!);
    }

    public override StringMenuInteractionData Data { get; }
}
