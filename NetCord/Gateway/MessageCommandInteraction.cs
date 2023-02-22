using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class MessageCommandInteraction : ApplicationCommandInteraction
{
    public MessageCommandInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
        Data = new(jsonModel.Data, client);
    }

    public override MessageCommandInteractionData Data { get; }
}
