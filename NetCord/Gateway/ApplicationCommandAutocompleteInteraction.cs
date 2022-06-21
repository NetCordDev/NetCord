using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class ApplicationCommandAutocompleteInteraction : Interaction
{
    public override ApplicationCommandAutocompleteInteractionData Data { get; }

    public ApplicationCommandAutocompleteInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
        Data = new(jsonModel.Data, jsonModel.GuildId, client);
    }
}