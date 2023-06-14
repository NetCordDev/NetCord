using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public abstract class ApplicationCommandInteraction : Interaction
{
    private protected ApplicationCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
    }

    public abstract override ApplicationCommandInteractionData Data { get; }
}
