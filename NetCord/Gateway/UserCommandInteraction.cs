using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class UserCommandInteraction : ApplicationCommandInteraction
{
    public override UserCommandInteractionData Data { get; }

    public UserCommandInteraction(JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }
}
