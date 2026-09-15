using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a custom (user-uploaded) emoji.
/// </summary>
public abstract class CustomEmoji(JsonEmoji jsonModel, RestClient client) : Emoji(jsonModel), ISpanFormattable
{
    /// <summary>
    /// The emoji's unique ID.
    /// </summary>
    public ulong Id => jsonModel.Id.GetValueOrDefault();

    /// <summary>
    /// The user that uploaded the emoji.
    /// </summary>
    public User? Creator { get; } = jsonModel.Creator is { } creator ? new(creator, client) : null;

    /// <summary>
    /// Whether this emoji must be wrapped in colons.
    /// </summary>
    public bool? RequireColons => jsonModel.RequireColons;

    /// <summary>
    /// Whether the emoji is managed by an application.
    /// </summary>
    /// <remarks>
    /// Managed emoji can only be created by apps with the <see cref="ApplicationFlags.ManagedEmoji"/> flag set.
    /// </remarks>
    public bool? Managed => jsonModel.Managed;

    /// <summary>
    /// Whether the emoji is available for use. Can be <see langword="false"/> if server boosts are lost.
    /// </summary>
    public bool? Available => jsonModel.Available;

    /// <summary>
    /// Returns an image representation of the emoji.
    /// </summary>
    public ImageUrl GetImageUrl(ImageFormat format) => ImageUrl.CustomEmoji(Id, format);

    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return string.Create(formatProvider, Animated ? $"<a:{Name}:{Id}>" : $"<:{Name}:{Id}>");
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        return Animated 
            ? destination.TryWrite(provider, $"<a:{Name}:{Id}>", out charsWritten)
            : destination.TryWrite(provider, $"<:{Name}:{Id}>", out charsWritten);
    }
}
