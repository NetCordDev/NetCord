using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a user as a member of the guild indicated by the <see cref="GuildId"/>.
/// </summary>
public partial class GuildUser(JsonGuildUser jsonModel, ulong guildId, RestClient client) : PartialGuildUser(jsonModel, client)
{
    /// <summary>
    /// The ID of the guild the <see cref="GuildUser"/> object belongs to.
    /// </summary>
    public ulong GuildId { get; } = guildId;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's guild avatar.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated avatars).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the user's guild avatar. If the user does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetGuildAvatarUrl(ImageFormat? format = null) => GuildAvatarHash is string hash ? ImageUrl.GuildUserAvatar(GuildId, Id, hash, format) : null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's guild avatar decoration.
    /// </summary>
    /// <returns>An <see cref="ImageUrl"/> pointing to the user's guild avatar decoration. If the user does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetGuildAvatarDecorationUrl() => GuildAvatarDecorationData is { Hash: var hash } ? ImageUrl.AvatarDecoration(hash) : null;

    /// <summary>
    /// Applies a timeout to the <see cref="GuildUser"/> for a specified <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="until">How long to time the <see cref="GuildUser"/> out for, specified as the time to wait until.</param>
    /// <param name="properties">Additional properties to apply to the REST request.</param>
    /// <returns> A <see cref="GuildUser"/> object updated with the new timeout. </returns>
    public Task<GuildUser> TimeOutAsync(DateTimeOffset until, RestRequestProperties? properties = null) => ModifyAsync(u => u.TimeOutUntil = until, properties);

    /// <summary>
    /// Returns a <see cref="GuildUserInfo"/> object representing the <see cref="GuildUser"/>.
    /// </summary>
    /// <param name="properties">Additional properties to apply to the REST request.</param>
    /// <exception cref="EntityNotFoundException"/>
    public async Task<GuildUserInfo> GetInfoAsync(RestRequestProperties? properties = null)
    {
        await foreach (var info in _client.SearchGuildUsersAsync(GuildId, new() { Limit = 1, AndQuery = [new UserIdsGuildUsersSearchQuery([Id])] }, properties).ConfigureAwait(false))
            return info;

        throw new EntityNotFoundException("The user was not found.");
    }
}
