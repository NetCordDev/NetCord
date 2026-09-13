using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a text channel for private messages between two users.
/// </summary>
internal partial class DMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : TextChannelBase(jsonModel, client), IDMChannel
{
    /// <summary>
    /// A list of the users present in the private channel, indexed by their IDs.
    /// </summary>
    public IReadOnlyDictionary<ulong, User> Users { get; } = jsonModel.Users.ToDictionaryOrEmpty(u => u.Id, u => new User(u, client));

    /// <inheritdoc cref="IPinnableChannel.LastPin" path="/summary" />
    public DateTimeOffset? LastPin => throw new NotImplementedException();
}
