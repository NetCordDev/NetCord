using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a role in a guild.
/// </summary>
public partial class Role : ClientEntity, IJsonModel<JsonRole>
{
    JsonRole IJsonModel<JsonRole>.JsonModel => _jsonModel;
    private readonly JsonRole _jsonModel;

    /// <summary>
    /// The <see cref="Role"/>'s ID.
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The name of the <see cref="Role"/>.
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// The <see cref="Role"/>'s colors.
    /// </summary>
    public RoleColors Colors { get; }

    /// <summary>
    /// Whether this <see cref="Role"/> causes members with it to be displayed in a separate section in the guild member list.
    /// </summary>
    public bool Hoist => _jsonModel.Hoist;

    /// <summary>
    /// The <see cref="Role"/>'s icon hash.
    /// </summary>
    public string? IconHash => _jsonModel.IconHash;

    /// <summary>
    /// The <see cref="Role"/>'s Unicode emoji.
    /// </summary>
    public string? UnicodeEmoji => _jsonModel.UnicodeEmoji;

    /// <summary>
    /// The raw position of this <see cref="Role"/>.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Position"/> to get a properly comparable and sortable position value.
    /// </remarks>
    public int RawPosition => _jsonModel.Position;

    /// <summary>
    /// The position of this <see cref="Role"/> for sorting and comparing.
    /// </summary>
    public RolePosition Position => new(RawPosition, Id);

    /// <summary>
    /// The permission bit set for this <see cref="Role"/>.
    /// </summary>
    public Permissions Permissions => _jsonModel.Permissions;

    /// <summary>
    /// Whether this <see cref="Role"/> is managed by an integration.
    /// </summary>
    public bool Managed => _jsonModel.Managed;

    /// <summary>
    /// Whether this <see cref="Role"/> is mentionable.
    /// </summary>
    public bool Mentionable => _jsonModel.Mentionable;

    /// <summary>
    /// The tags this <see cref="Role"/> has.
    /// </summary>
    public RoleTags? Tags { get; }

    /// <summary>
    /// The <see cref="Role"/>'s flags combined as a bitfield.
    /// </summary>
    public RoleFlags Flags => _jsonModel.Flags;

    /// <summary>
    /// The ID of the guild this <see cref="Role"/> belongs to.
    /// </summary>
    public ulong GuildId { get; }

    public Role(JsonRole jsonModel, ulong guildId, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        Colors = new(jsonModel.Colors);

        if (jsonModel.Tags is { } tags)
            Tags = new(tags);

        GuildId = guildId;
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="Role"/>'s icon.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the role's icon. If the role does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetIconUrl(ImageFormat format) => IconHash is string hash ? ImageUrl.RoleIcon(Id, hash, format) : null;

    public override string ToString() => $"<@&{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatRole(destination, out charsWritten, Id);
}

/// <summary>
/// Represents the colors of a <see cref="Role"/>.
/// </summary>
public class RoleColors(JsonRoleColors jsonModel) : IJsonModel<JsonRoleColors>
{
    JsonRoleColors IJsonModel<JsonRoleColors>.JsonModel => jsonModel;

    /// <summary>
    /// The primary color for the <see cref="Role"/>.
    /// </summary>
    public Color PrimaryColor => jsonModel.PrimaryColor;

    /// <summary>
    /// The secondary color for the <see cref="Role"/>. This will make the role a gradient between the other provided colors.
    /// </summary>
    /// <remarks>
    /// Requires the guild to have the <c>ENHANCED_ROLE_COLORS</c> guild feature.
    /// </remarks>
    public Color? SecondaryColor => jsonModel.SecondaryColor;

    /// <summary>
    /// The tertiary color for the <see cref="Role"/>. This will turn the gradient into a holographic style.
    /// </summary>
    /// <remarks>
    /// Requires the guild to have the <c>ENHANCED_ROLE_COLORS</c> guild feature.
    /// </remarks>
    public Color? TertiaryColor => jsonModel.TertiaryColor;
}

/// <summary>
/// Represents the tags associated with a <see cref="Role"/>.
/// </summary>
public class RoleTags(JsonRoleTags jsonModel) : IJsonModel<JsonRoleTags>
{
    JsonRoleTags IJsonModel<JsonRoleTags>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the bot this <see cref="Role"/> belongs to.
    /// </summary>
    public ulong? BotId => jsonModel.BotId;

    /// <summary>
    /// The ID of the integration this <see cref="Role"/> belongs to.
    /// </summary>
    public ulong? IntegrationId => jsonModel.IntegrationId;

    /// <summary>
    /// Whether this is the guild's Booster role.
    /// </summary>
    public bool IsPremiumSubscriber => jsonModel.IsPremiumSubscriber;

    /// <summary>
    /// The ID of this <see cref="Role"/>'s subscription SKU and listing.
    /// </summary>
    public ulong? SubscriptionListingId => jsonModel.SubscriptionListingId;

    /// <summary>
    /// Whether this <see cref="Role"/> is available for purchase.
    /// </summary>
    public bool IsAvailableForPurchase => jsonModel.IsAvailableForPurchase;

    /// <summary>
    /// Whether this <see cref="Role"/> is a guild's linked role.
    /// </summary>
    public bool GuildConnections => jsonModel.GuildConnections;
}
