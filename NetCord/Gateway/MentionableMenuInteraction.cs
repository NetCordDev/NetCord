using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class MentionableMenuInteraction : EntityMenuInteraction
{
    public MentionableMenuInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
    }
}
