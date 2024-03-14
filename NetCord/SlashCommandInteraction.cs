using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class SlashCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RestRequestProperties?, Task> sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override SlashCommandInteractionData Data { get; } = new(jsonModel.Data!, jsonModel.GuildId, client);
}
