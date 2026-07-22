using NetCord.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a <see cref="ThreadUser"/> with an included user object.
/// </summary>
public partial class GuildThreadUser(JsonThreadUser jsonModel, RestClient client) : ThreadUser(jsonModel, client)
{
    /// <summary>
    /// The thread user's <see cref="PartialGuildUser"/> object.
    /// </summary>

    public PartialGuildUser GuildUser { get; } = new(jsonModel.GuildUser!, client);
}
