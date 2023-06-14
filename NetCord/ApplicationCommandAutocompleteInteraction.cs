using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class ApplicationCommandAutocompleteInteraction : Interaction
{
    public ApplicationCommandAutocompleteInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }

    public override ApplicationCommandAutocompleteInteractionData Data { get; }
}
