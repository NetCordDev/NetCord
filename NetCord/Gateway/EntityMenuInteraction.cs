using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public abstract class EntityMenuInteraction : Interaction
{
    public EntityMenuInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
        Data = new(jsonModel.Data, jsonModel.GuildId, client);
        jsonModel.Message.GuildId = jsonModel.GuildId;
        Message = new(jsonModel.Message, guild, channel, client);
    }

    public override EntityMenuInteractionData Data { get; }
    public Message Message { get; }
}
