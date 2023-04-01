using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class StringMenuInteraction : Interaction
{
    public override StringMenuInteractionData Data { get; }

    public Message Message { get; }

    public StringMenuInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
        Data = new(jsonModel.Data!);
        jsonModel.Message!.GuildId = jsonModel.GuildId;
        Message = new(jsonModel.Message, guild, channel, client);
    }
}
