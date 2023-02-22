using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public abstract class ApplicationCommandInteraction : Interaction
{
    public abstract override ApplicationCommandInteractionData Data { get; }

    protected ApplicationCommandInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
    }
}
