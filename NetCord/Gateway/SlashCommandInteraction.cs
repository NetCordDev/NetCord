using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class SlashCommandInteraction : ApplicationCommandInteraction
{
    public override SlashCommandInteractionData Data { get; }

    public SlashCommandInteraction(JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }
}
