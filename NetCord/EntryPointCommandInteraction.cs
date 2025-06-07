using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class EntryPointCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override EntryPointCommandInteractionData Data { get; } = new(jsonModel.Data!);
}
