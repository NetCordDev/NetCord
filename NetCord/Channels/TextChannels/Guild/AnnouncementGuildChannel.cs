using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a channel that users can follow and crosspost from into their own servers. Formerly known as news channels.
/// </summary>
public partial class AnnouncementGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : TextGuildChannel(jsonModel, guildId, client)
{
}
