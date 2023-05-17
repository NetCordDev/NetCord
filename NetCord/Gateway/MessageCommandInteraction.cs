using NetCord.Rest;

namespace NetCord.Gateway;

public class MessageCommandInteraction : ApplicationCommandInteraction
{
    public MessageCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, client);
    }

    public override MessageCommandInteractionData Data { get; }
}
