using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class UserCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override UserCommandInteractionData Data { get; } = new(jsonModel.Data!, jsonModel.GuildId, client);
}
