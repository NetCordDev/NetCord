using System.Runtime.CompilerServices;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class Role : ClientEntity, IJsonModel<JsonRole>, IComparable<Role>
{
    JsonRole IJsonModel<JsonRole>.JsonModel => _jsonModel;
    private readonly JsonRole _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public Color Color => _jsonModel.Color;

    public bool Hoist => _jsonModel.Hoist;

    public string? IconHash => _jsonModel.IconHash;

    public string? UnicodeEmoji => _jsonModel.UnicodeEmoji;

    public int Position => _jsonModel.Position;

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

    public override string ToString() => $"<@&{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatRole(destination, out charsWritten, Id);

    public int CompareTo(Role? other)
    {
        if (other is null)
            return 1;

        return Position.CompareTo(other.Position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Role left, Role right) => left.Position > right.Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Role left, Role right) => left.Position < right.Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Role left, Role right) => left.Position >= right.Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Role left, Role right) => left.Position <= right.Position;
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
