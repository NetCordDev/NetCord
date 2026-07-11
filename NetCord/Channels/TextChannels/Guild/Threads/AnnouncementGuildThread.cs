using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a thread within a <see cref="AnnouncementGuildChannel"/>.
/// </summary>
public partial class AnnouncementGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client)
{
}
