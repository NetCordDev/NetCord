using NetCord.JsonModels;

namespace NetCord;

public class SlashCommandInteraction : ApplicationCommandInteraction
{
    public SlashCommandInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
        Data = new(jsonModel.Data, jsonModel.GuildId, client);
    }

    public override SlashCommandInteractionData Data { get; }
}