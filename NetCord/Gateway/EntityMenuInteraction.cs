using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public abstract class EntityMenuInteraction : Interaction
{
    public override EntityMenuInteractionData Data { get; }

    public Message Message { get; }

    protected EntityMenuInteraction(JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
        jsonModel.Message!.GuildId = jsonModel.GuildId;
        Message = new(jsonModel.Message, guild, Channel, client);
    }
}
