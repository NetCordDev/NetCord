using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class Role : ClientEntity, IJsonModel<JsonRole>
{
    JsonRole IJsonModel<JsonRole>.JsonModel => _jsonModel;
    private readonly JsonRole _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public RoleColors Colors { get; }

    public bool Hoist => _jsonModel.Hoist;

    public string? IconHash => _jsonModel.IconHash;

    public string? UnicodeEmoji => _jsonModel.UnicodeEmoji;

    public int RawPosition => _jsonModel.Position;

    public RolePosition Position => new(RawPosition, Id);

    public Permissions Permissions => _jsonModel.Permissions;

    public bool Managed => _jsonModel.Managed;

    public bool Mentionable => _jsonModel.Mentionable;

    public RoleTags? Tags { get; }

    public RoleFlags Flags => _jsonModel.Flags;

    public ulong GuildId { get; }

    public Role(JsonRole jsonModel, ulong guildId, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        Colors = new(jsonModel.Colors);

        if (jsonModel.Tags is { } tags)
            Tags = new(tags);

        GuildId = guildId;
    }

    public ImageUrl? GetIconUrl(ImageFormat format) => IconHash is string hash ? ImageUrl.RoleIcon(Id, hash, format) : null;

    public override string ToString() => $"<@&{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatRole(destination, out charsWritten, Id);
}

public class RoleColors(JsonRoleColors jsonModel) : IJsonModel<JsonRoleColors>
{
    JsonRoleColors IJsonModel<JsonRoleColors>.JsonModel => jsonModel;

    public Color PrimaryColor => jsonModel.PrimaryColor;
    public Color? SecondaryColor => jsonModel.SecondaryColor;
    public Color? TertiaryColor => jsonModel.TertiaryColor;
}

public class RoleTags(JsonRoleTags jsonModel) : IJsonModel<JsonRoleTags>
{
    JsonRoleTags IJsonModel<JsonRoleTags>.JsonModel => jsonModel;

    public ulong? BotId => jsonModel.BotId;

    public ulong? IntegrationId => jsonModel.IntegrationId;

    public bool IsPremiumSubscriber => jsonModel.IsPremiumSubscriber;

    public ulong? SubscriptionListingId => jsonModel.SubscriptionListingId;

    public bool IsAvailableForPurchase => jsonModel.IsAvailableForPurchase;

    public bool GuildConnections => jsonModel.GuildConnections;
}
