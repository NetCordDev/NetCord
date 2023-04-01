using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class ModalSubmitInteraction : Interaction
{
    public override ModalSubmitInteractionData Data { get; }

    /// <summary>
    /// Available if the modal was opened in response to a component interaction.
    /// </summary>
    public Message? Message { get; }

    public ModalSubmitInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
        Data = new(jsonModel.Data!);
        if (jsonModel.Message != null)
        {
            jsonModel.Message.GuildId = jsonModel.GuildId;
            Message = new(jsonModel.Message, guild, channel, client);
        }
    }
}
