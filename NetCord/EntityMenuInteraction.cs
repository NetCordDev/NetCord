using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public abstract class EntityMenuInteraction : MessageComponentInteraction
{
    private protected EntityMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RestRequestProperties?, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
        Data = new(jsonModel.Data!, jsonModel.GuildId, client);
    }

    public override EntityMenuInteractionData Data { get; }
}
