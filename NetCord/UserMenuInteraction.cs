using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class UserMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : EntityMenuInteraction(jsonModel, guild, sendResponseAsync, client)
{
}
