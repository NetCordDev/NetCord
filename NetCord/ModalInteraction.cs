using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class ModalInteraction : ComponentInteraction
{
    public ModalInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
        var message = jsonModel.Message;
        if (message is not null)
        {
            message.GuildId = jsonModel.GuildId;
            Message = new(message, guild, Channel, client);
        }

        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }

    /// <summary>
    /// Available if the modal was opened in response to a component interaction.
    /// </summary>
    public Message? Message { get; }

    public override ModalInteractionData Data { get; }
}

public class ModalInteractionData : ComponentInteractionData
{
    public ModalInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel)
    {
        var jsonResolvedData = jsonModel.ResolvedData;

        InteractionResolvedData? resolvedData;
        if (jsonResolvedData is null)
            resolvedData = null;
        else
            ResolvedData = resolvedData = new(jsonResolvedData, guildId, client);

        Components = jsonModel.Components!.Select(c => IModalComponent.CreateFromJson(c, resolvedData)).ToArray();
    }

    public IReadOnlyList<IModalComponent> Components { get; }

    public InteractionResolvedData? ResolvedData { get; }
}
