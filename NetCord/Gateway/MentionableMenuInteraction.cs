using NetCord.Rest;

namespace NetCord.Gateway;

public class MentionableMenuInteraction : EntityMenuInteraction
{
    public MentionableMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
    }
}
