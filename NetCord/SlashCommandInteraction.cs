using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class SlashCommandInteraction : ApplicationCommandInteraction
{
    public SlashCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }

    public override SlashCommandInteractionData Data { get; }
}
