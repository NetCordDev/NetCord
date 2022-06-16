using NetCord.JsonModels;

namespace NetCord;

public class ModalSubmitInteraction : Interaction
{
    public override ModalSubmitInteractionData Data { get; }

    public ModalSubmitInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
        Data = new(jsonModel.Data);
    }
}