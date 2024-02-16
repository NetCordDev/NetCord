using NetCord.Rest;

namespace NetCord;

public partial class AnnouncementGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : TextGuildChannel(jsonModel, guildId, client)
{
}
