using NetCord.Rest;

namespace NetCord.Gateway;

public class ModalSubmitInteraction : Interaction
{
    public ModalSubmitInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!);
        if (jsonModel.Message != null)
        {
            jsonModel.Message.GuildId = jsonModel.GuildId;
            Message = new(jsonModel.Message, guild, Channel, client);
        }
    }

    public override ModalSubmitInteractionData Data { get; }

    /// <summary>
    /// Available if the modal was opened in response to a component interaction.
    /// </summary>
    public Message? Message { get; }
}
