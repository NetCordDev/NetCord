using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a base Discord emoji.
/// </summary>
public abstract class Emoji(JsonEmoji jsonModel)
{
    /// <summary>
    /// The emoji's name.
    /// </summary>
    public string Name { get; } = jsonModel.Name!;

    /// <summary>
    /// Whether the emoji is animated.
    /// </summary>
    public bool? Animated { get; } = jsonModel.Animated;

    public abstract override string ToString();

    public static Emoji Create(JsonEmoji jsonModel, ulong guildId, RestClient client)
    {
        return jsonModel.Id.HasValue
            ? new GuildEmoji(jsonModel, guildId, client)
            : new StandardEmoji(jsonModel);
    }
}

public class StandardEmoji(JsonEmoji jsonModel) : Emoji(jsonModel)
{
    public override string ToString() => Name;
}

/// <summary>
/// Represents a custom (user-uploaded) emoji.
/// </summary>
public abstract class CustomEmoji(JsonEmoji jsonModel, RestClient client) : Emoji(jsonModel), ISpanFormattable
{
    /// <summary>
    /// The emoji's unique ID.
    /// </summary>
    public ulong Id { get; } = jsonModel.Id.GetValueOrDefault();

    /// <summary>
    /// The user that uploaded the emoji.
    /// </summary>
    public User? Creator { get; } = jsonModel.Creator is { } creator ? new(creator, client) : null;

    /// <summary>
    /// Whether this emoji must be wrapped in colons.
    /// </summary>
    public bool? RequireColons { get; } = jsonModel.RequireColons;

    /// <summary>
    /// Whether the emoji is managed by an application.
    /// </summary>
    /// <remarks>
    /// Managed emoji can only be created by apps with the <see cref="ApplicationFlags.ManagedEmoji"/> flag set.
    /// </remarks>
    public bool? Managed { get; } = jsonModel.Managed;

    /// <summary>
    /// Whether the emoji is available for use. Can be <see langword="false"/> if server boosts are lost.
    /// </summary>
    public bool? Available { get; } = jsonModel.Available;

    /// <summary>
    /// Returns an image representation of the emoji.
    /// </summary>
    public ImageUrl GetImageUrl(ImageFormat format) => ImageUrl.CustomEmoji(Id, format);

    public override string ToString() => Animated.GetValueOrDefault() ? $"<a:{Name}:{Id}>" : $"<:{Name}:{Id}>";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var name = Name;
        if (Animated.GetValueOrDefault())
        {
            if (destination.Length < 6 + name.Length || !Id.TryFormat(destination[(4 + name.Length)..^1], out int length))
            {
                charsWritten = 0;
                return false;
            }

            "<a:".CopyTo(destination);
            name.CopyTo(destination[3..]);
            destination[3 + name.Length] = ':';
            destination[4 + name.Length + length] = '>';
            charsWritten = 5 + name.Length + length;
            return true;
        }
        else
        {
            if (destination.Length < 5 + name.Length || !Id.TryFormat(destination[(3 + name.Length)..^1], out int length))
            {
                charsWritten = 0;
                return false;
            }

            "<:".CopyTo(destination);
            name.CopyTo(destination[2..]);
            destination[2 + name.Length] = ':';
            destination[3 + name.Length + length] = '>';
            charsWritten = 4 + name.Length + length;
            return true;
        }
    }
}

/// <summary>
/// Represents a custom application emoji.
/// </summary>
public partial class ApplicationEmoji(JsonEmoji jsonModel, ulong applicationId, RestClient client) : CustomEmoji(jsonModel, client)
{
    /// <summary>
    /// The ID corresponding to the emoji's parent application.
    /// </summary>
    public ulong ApplicationId { get; } = applicationId;
}

/// <summary>
/// Represents a custom guild emoji.
/// </summary>
public partial class GuildEmoji(JsonEmoji jsonModel, ulong guildId, RestClient client) : CustomEmoji(jsonModel, client)
{
    /// <summary>
    /// A list of roles allowed to use this emoji.
    /// </summary>
    public IReadOnlyList<ulong>? AllowedRoles { get; } = jsonModel.AllowedRoles;

    /// <summary>
    /// The ID corresponding to the emoji's parent guild.
    /// </summary>
    public ulong GuildId { get; } = guildId;
}
