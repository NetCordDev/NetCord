using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class ApplicationCommandAutocompleteInteraction : Interaction
{
    public override ApplicationCommandAutocompleteInteractionData Data { get; }

    public ApplicationCommandAutocompleteInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }
}
