using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class MessageCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RestRequestProperties?, CancellationToken, Task> sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override MessageCommandInteractionData Data { get; } = new(jsonModel.Data!, client);
}
