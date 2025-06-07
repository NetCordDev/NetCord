using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class MentionableMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : EntityMenuInteraction(jsonModel, guild, sendResponseAsync, client)
{
}
