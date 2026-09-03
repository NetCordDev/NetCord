using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a text channel for private messages between two users.
/// </summary>
public partial class DMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : TextChannel(jsonModel, client)
{
    /// <summary>
    /// A list of the users present in the private channel, indexed by their IDs.
    /// </summary>
    public IReadOnlyDictionary<ulong, User> Users { get; } = jsonModel.Users.ToDictionaryOrEmpty(u => u.Id, u => new User(u, client));

    public static new DMChannel CreateFromJson(JsonModels.JsonChannel jsonModel, RestClient client)
    {
        return jsonModel.Type switch
        {
            ChannelType.DMChannel => new DMChannel(jsonModel, client),
            ChannelType.GroupDMChannel => new GroupDMChannel(jsonModel, client),
            _ => new UnknownDMChannel(jsonModel, client),
        };
    }
}
