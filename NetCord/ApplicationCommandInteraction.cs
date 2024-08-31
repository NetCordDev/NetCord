using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public abstract class ApplicationCommandInteraction : Interaction
{
    private protected ApplicationCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RestRequestProperties?, CancellationToken, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
    }

    public abstract override ApplicationCommandInteractionData Data { get; }
}
