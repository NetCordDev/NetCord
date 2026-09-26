using NetCord.JsonModels;

namespace NetCord;

public class UserPrimaryGuild(JsonUserPrimaryGuild jsonModel) : IJsonModel<JsonUserPrimaryGuild>
{
    JsonUserPrimaryGuild IJsonModel<JsonUserPrimaryGuild>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the user's primary guild.
    /// </summary>
    public ulong? IdentityGuildId => jsonModel.IdentityGuildId;

    /// <summary>
    /// Whether the user is displaying the primary guild's server tag. This can be <see langword="null"/> if the system clears the identity, e.g. because the server no longer supports tags.
    /// </summary>
    public bool? IdentityEnabled => jsonModel.IdentityEnabled;

    /// <summary>
    /// The text of the user's server tag. Limited to 4 characters.
    /// </summary>
    public string? Tag => jsonModel.Tag;

    /// <summary>
    /// The server tag badge hash.
    /// </summary>
    public string? BadgeHash => jsonModel.BadgeHash;

    /// <summary>
    /// Whether the primary guild has a set server tag badge.
    /// </summary>
    public bool HasBadge => BadgeHash is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the server tag badge.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the server tag badge. If the server tag does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetBadgeUrl(ImageFormat format) => BadgeHash is string hash ? ImageUrl.GuildTagBadge(IdentityGuildId.GetValueOrDefault(), hash, format) : null;
}
