using NetCord.Rest;

namespace NetCord.Gateway;

public class SlashCommandInteraction : ApplicationCommandInteraction
{
    public SlashCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }

    public override SlashCommandInteractionData Data { get; }
}
