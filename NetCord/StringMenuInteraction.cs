using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class StringMenuInteraction : MessageComponentInteraction
{
    public StringMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!);
    }

    public override StringMenuInteractionData Data { get; }
}
