using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class ButtonInteraction : Interaction
{
    public override ButtonInteractionData Data { get; }

    public Message Message { get; }

    public ButtonInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
        Data = new(jsonModel.Data);
        Message = new(jsonModel.Message with { GuildId = jsonModel.GuildId }, guild, channel, client);
    }
}
