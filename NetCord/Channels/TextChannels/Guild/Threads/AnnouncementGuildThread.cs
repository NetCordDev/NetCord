using NetCord.Rest;

namespace NetCord;

public partial class AnnouncementGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client)
{
}
