using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class ModalSubmitInteraction : Interaction
{
    public ModalSubmitInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        var message = jsonModel.Message;
        if (message is not null)
        {
            message.GuildId = jsonModel.GuildId;
            Message = new(message, guild, Channel, client);
        }

        Data = new(jsonModel.Data!);
    }

    /// <summary>
    /// Available if the modal was opened in response to a component interaction.
    /// </summary>
    public Message? Message { get; }

    public override ModalSubmitInteractionData Data { get; }
}
