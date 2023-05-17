using NetCord.Rest;

namespace NetCord.Gateway;

public class ChannelMenuInteraction : EntityMenuInteraction
{
    public ChannelMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
    }
}
