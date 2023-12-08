using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class MessageCommandInteraction : ApplicationCommandInteraction
{
    public MessageCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
        Data = new(jsonModel.Data!, client);
    }

    public override MessageCommandInteractionData Data { get; }
}
