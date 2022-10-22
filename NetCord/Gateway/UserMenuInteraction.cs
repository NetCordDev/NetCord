using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class UserMenuInteraction : EntityMenuInteraction
{
    public UserMenuInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
    }
}

public class RoleMenuInteraction : EntityMenuInteraction
{
    public RoleMenuInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
    }
}

public class MentionableMenuInteraction : EntityMenuInteraction
{
    public MentionableMenuInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
    }
}

public class ChannelMenuInteraction : EntityMenuInteraction
{
    public ChannelMenuInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
    }
}
