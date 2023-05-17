using NetCord.Rest;

namespace NetCord.Gateway;

public class ButtonInteraction : Interaction
{
    public override ButtonInteractionData Data { get; }

    public Message Message { get; }

    public ButtonInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!);
        jsonModel.Message!.GuildId = jsonModel.GuildId;
        Message = new(jsonModel.Message, guild, Channel, client);
    }
}
