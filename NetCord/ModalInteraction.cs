using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class ModalInteraction : ComponentInteraction
{
    public ModalInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RestRequestProperties?, CancellationToken, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
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

    public override ModalInteractionData Data { get; }
}
