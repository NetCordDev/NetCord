using NetCord.Rest;

namespace NetCord.Gateway;

public class UserCommandInteraction : ApplicationCommandInteraction
{
    public UserCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }

    public override UserCommandInteractionData Data { get; }
}
