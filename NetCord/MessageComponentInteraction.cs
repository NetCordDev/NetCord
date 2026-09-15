using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract class MessageComponentInteraction : ComponentInteraction
{
    private protected MessageComponentInteraction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) 
        : base(jsonModel, guild, sendResponseAsync, client)
    {
        var message = jsonModel.Message!;
        message.GuildId = jsonModel.GuildId;
        Message = new Message(message, guild, Channel, client);
    }

    public Message Message { get; }

    public abstract override MessageComponentInteractionData Data { get; }
}

public abstract class MessageComponentInteractionData(JsonInteractionData jsonModel) : ComponentInteractionData(jsonModel)
{
    public int Id => (int)_jsonModel.Id.GetValueOrDefault();

    public ComponentType ComponentType => _jsonModel.ComponentType.GetValueOrDefault();
}
