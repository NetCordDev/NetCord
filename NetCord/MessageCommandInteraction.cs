using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class MessageCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override MessageCommandInteractionData Data { get; } = new(jsonModel.Data!, client);
}

public class MessageCommandInteractionData(JsonModels.JsonInteractionData jsonModel, RestClient client) : ApplicationCommandInteractionData(jsonModel)
{
    public RestMessage TargetMessage { get; } = new(jsonModel.ResolvedData!.Messages![jsonModel.TargetId.GetValueOrDefault()], client);
}
