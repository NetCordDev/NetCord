using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class AutocompleteInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : Interaction(jsonModel, guild, sendResponseAsync, client)
{
    public override AutocompleteInteractionData Data { get; } = new(jsonModel.Data!, jsonModel.GuildId, client);
}
