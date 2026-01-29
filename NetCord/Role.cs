using System.Diagnostics.CodeAnalysis;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class Role : ClientEntity, IJsonModel<JsonRole>
{
    JsonRole IJsonModel<JsonRole>.JsonModel => _jsonModel;
    private readonly JsonRole _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public Color Color => _jsonModel.Color;

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

        var tags = jsonModel.Tags;
        if (tags is not null)
            Tags = new(tags);

        GuildId = guildId;
    }

    public ImageUrl? GetIconUrl(ImageFormat format) => IconHash is string hash ? ImageUrl.RoleIcon(Id, hash, format) : null;

    public override string ToString() => $"<@&{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatRole(destination, out charsWritten, Id);
}

public readonly struct RolePosition : IComparable<RolePosition>, IEquatable<RolePosition>
{
    private readonly int _position;
    private readonly ulong _roleId;

    internal RolePosition(int position, ulong roleId)
    {
        _position = position;
        _roleId = roleId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_position, _roleId);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is RolePosition other && Equals(other);
    }

    public bool Equals(RolePosition other)
    {
        return _position == other._position && _roleId == other._roleId;
    }

    public int CompareTo(RolePosition other)
    {
        var positionCompare = _position.CompareTo(other._position);
        return positionCompare is 0 ? other._roleId.CompareTo(_roleId) : positionCompare;
    }

    public static bool operator ==(RolePosition left, RolePosition right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RolePosition left, RolePosition right)
    {
        return !left.Equals(right);
    }

    public static bool operator >(RolePosition left, RolePosition right)
    {
        return left._position > right._position || (left._position == right._position && left._roleId < right._roleId);
    }

    public static bool operator <(RolePosition left, RolePosition right)
    {
        return left._position < right._position || (left._position == right._position && left._roleId > right._roleId);
    }

    public static bool operator >=(RolePosition left, RolePosition right)
    {
        return left._position > right._position || (left._position == right._position && left._roleId <= right._roleId);
    }

    public static bool operator <=(RolePosition left, RolePosition right)
    {
        return left._position < right._position || (left._position == right._position && left._roleId >= right._roleId);
    }
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
