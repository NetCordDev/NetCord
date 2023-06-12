using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class StringMenuInteraction : Interaction
{
    public StringMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!);
        jsonModel.Message!.GuildId = jsonModel.GuildId;
        Message = new(jsonModel.Message, guild, Channel, client);
    }

    public override StringMenuInteractionData Data { get; }

    public Message Message { get; }
}
