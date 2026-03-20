using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class EntryPointCommandInteraction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override EntryPointCommandInteractionData Data { get; } = new(jsonModel.Data!);
}

public class EntryPointCommandInteractionData(JsonInteractionData jsonModel) : ApplicationCommandInteractionData(jsonModel)
{
}
