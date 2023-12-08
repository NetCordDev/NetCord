using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract class MessageComponentInteraction : Interaction
{
    private protected MessageComponentInteraction(JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
        var message = jsonModel.Message!;
        message.GuildId = jsonModel.GuildId;
        Message = new(message, guild, Channel, client);
    }

    public Message Message { get; }

    public abstract override MessageComponentInteractionData Data { get; }
}
