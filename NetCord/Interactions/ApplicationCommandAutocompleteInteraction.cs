using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class ApplicationCommandAutocompleteInteraction : Interaction
{
    public override ApplicationCommandAutocompleteInteractionData Data { get; }

    public ApplicationCommandAutocompleteInteraction(JsonInteraction jsonModel, GatewayClient client) : base(jsonModel, client)
    {
        Data = new(jsonModel.Data, jsonModel.GuildId, client.Rest);
    }
}