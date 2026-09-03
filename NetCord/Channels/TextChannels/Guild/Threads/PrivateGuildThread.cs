using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a <see cref="GuildThread"/> only accessible to a subset of users.
/// </summary>
public partial class PrivateGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client)
{
}
