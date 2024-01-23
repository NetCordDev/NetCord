using NetCord.Rest;

namespace NetCord;

public partial class AnnouncementGuildChannel : TextGuildChannel
{
    public AnnouncementGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, guildId, client)
    {
    }
}
