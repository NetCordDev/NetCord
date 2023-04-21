using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public abstract class ApplicationCommandInteraction : Interaction
{
    public abstract override ApplicationCommandInteractionData Data { get; }

    protected ApplicationCommandInteraction(JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
    }
}
