using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a custom (user-uploaded) emoji.
/// </summary>
public abstract class CustomEmoji : Emoji, ISpanFormattable
{
    private protected RestClient _client;

    public CustomEmoji(JsonEmoji jsonModel, RestClient client) : base(jsonModel)
    {
        _client = client;

        var creator = jsonModel.Creator;
        if (creator is not null)
            Creator = new(creator, client);
    }

    /// <summary>
    /// The emoji's unique ID.
    /// </summary>
    public ulong Id => _jsonModel.Id.GetValueOrDefault();

    /// <summary>
    /// The user that uploaded the emoji.
    /// </summary>
    public User? Creator { get; }

    /// <summary>
    /// Whether this emoji must be wrapped in colons.
    /// </summary>
    public bool? RequireColons => _jsonModel.RequireColons;

    /// <summary>
    /// Whether the emoji is managed by an application.
    /// </summary>
    /// <remarks>
    /// Managed emoji can only be created by apps with the <see cref="ApplicationFlags.ManagedEmoji"/> flag set.
    /// </remarks>
    public bool? Managed => _jsonModel.Managed;

    /// <summary>
    /// Whether the emoji is available for use. Can be <see langword="false"/> if server boosts are lost.
    /// </summary>
    public bool? Available => _jsonModel.Available;

    /// <summary>
    /// Returns an image representation of the emoji.
    /// </summary>
    public ImageUrl GetImageUrl(ImageFormat format) => ImageUrl.CustomEmoji(Id, format);

    public override string ToString() => Animated ? $"<a:{Name}:{Id}>" : $"<:{Name}:{Id}>";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var name = Name;
        if (Animated)
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
