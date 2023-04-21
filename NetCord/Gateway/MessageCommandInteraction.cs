using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class MessageCommandInteraction : ApplicationCommandInteraction
{
    public override MessageCommandInteractionData Data { get; }

    public MessageCommandInteraction(JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, client);
    }
}
