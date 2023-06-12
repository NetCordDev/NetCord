using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public abstract class ApplicationCommandInteraction : Interaction
{
    public abstract override ApplicationCommandInteractionData Data { get; }

    protected ApplicationCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
    }
}
