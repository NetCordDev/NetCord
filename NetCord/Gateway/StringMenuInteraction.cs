using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class StringMenuInteraction : Interaction
{
    public override StringMenuInteractionData Data { get; }

    public Message Message { get; }

    public StringMenuInteraction(JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!);
        jsonModel.Message!.GuildId = jsonModel.GuildId;
        Message = new(jsonModel.Message, guild, Channel, client);
    }
}
