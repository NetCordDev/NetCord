using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a thread within an <see cref="AnnouncementGuildChannel"/>.
/// </summary>
internal partial class AnnouncementGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client)
{
}
