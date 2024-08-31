using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class StringMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RestRequestProperties?, CancellationToken, Task> sendResponseAsync, RestClient client) : MessageComponentInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override StringMenuInteractionData Data { get; } = new(jsonModel.Data!);
}
