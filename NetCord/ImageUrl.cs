using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NetCord;

#pragma warning disable IDE0032 // Use auto property

/// <summary>
/// Represents a URL to an image hosted by Discord.
/// </summary>
public class ImageUrl : ISpanFormattable
{
    private readonly string _url;
    private readonly bool _supportsSize;

    private ImageUrl(string partialUrl, string extension, string baseUrl = Discord.CDNUrl, bool supportsSize = true) : this($"{baseUrl}{partialUrl}.{extension}", supportsSize)
    {
    }

    private ImageUrl(string url, bool supportsSize = true)
    {
        _url = url;
        _supportsSize = supportsSize;
    }

    /// <summary>
    /// Indicates whether this <see cref="ImageUrl"/> supports a size query parameter.
    /// </summary>
    public bool SupportsSize => _supportsSize;

    public override string ToString() => _url;

    /// <summary>
    /// Returns the <see cref="ImageUrl"/> with a specified size.
    /// </summary>
    /// <param name="size">The desired size of the image. Can be any power of two between 16 and 4096.</param>
    /// <returns>A string representation of the <see cref="ImageUrl"/> with the specified size.</returns>
    public virtual string ToString(int size)
    {
        if (!_supportsSize)
            ThrowSizeNotSupported();

        return $"{_url}?size={size}";
    }

    /// <summary>
    /// Formats the value of the current <see cref="ImageUrl"/> using the specified format.
    /// </summary>
    /// <param name="format">The format to use. If <see langword="null"/> or empty, returns the URL without size parameter. Otherwise, should be a numeric string representing the desired size.</param>
    /// <param name="formatProvider">The provider to use to format the value. This parameter is ignored.</param>
    /// <returns>A string representation of the <see cref="ImageUrl"/>.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
            return _url;

        if (!_supportsSize)
            ThrowSizeNotSupported();

        if (!IsSizeValid(format))
            ThrowInvalidFormat();

        return $"{_url}?size={format}";
    }

    /// <summary>
    /// Tries to format the value of the current <see cref="ImageUrl"/> into the provided span of characters.
    /// </summary>
    /// <param name="destination">The span in which to write the formatted value.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters that were written in <paramref name="destination"/>.</param>
    /// <param name="format">A span containing the format string. If empty, returns the URL without size parameter. Otherwise, should contain numeric characters representing the desired size.</param>
    /// <param name="provider">The provider to use to format the value. This parameter is ignored.</param>
    /// <returns><see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.</returns>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        if (format.IsEmpty)
        {
            var url = _url;
            if (!url.TryCopyTo(destination))
            {
                charsWritten = 0;
                return false;
            }

            charsWritten = url.Length;
            return true;
        }

        if (!_supportsSize)
            ThrowSizeNotSupported();

        if (!IsSizeValid(format))
            ThrowInvalidFormat();

        {
            var url = _url;
            var requiredLength = url.Length + format.Length + 6;
            if (destination.Length < requiredLength)
            {
                charsWritten = 0;
                return false;
            }

            url.CopyTo(destination);
            "?size=".CopyTo(destination[url.Length..]);
            format.CopyTo(destination[(url.Length + 6)..]);

            charsWritten = requiredLength;
            return true;
        }
    }

    private protected virtual bool IsSizeValid(ReadOnlySpan<char> format)
    {
        int length = format.Length;
        for (int i = 0; i < length; i++)
        {
            if (!char.IsAsciiDigit(format[i]))
                return false;
        }
        return true;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowSizeNotSupported()
    {
        throw new NotSupportedException("This image URL does not support setting size.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidFormat()
    {
        throw new FormatException("Format specifier was invalid.");
    }

    private static string GetExtension(string hash, ImageFormat? format)
    {
        return format.HasValue
            ? GetFormat(format.GetValueOrDefault())
            : hash.AsSpanFast() is ['a', '_', ..] ? "gif" : "png";
    }

    internal static string GetFormat(ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Jpeg => "jpg",
            ImageFormat.Png => "png",
            ImageFormat.WebP => "webp",
            ImageFormat.Gif => "gif",
            ImageFormat.Lottie => "json",
            _ => throw new System.ComponentModel.InvalidEnumArgumentException("Invalid image format.")
        };
    }

    internal static ReadOnlySpan<byte> GetFormatBytes(ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Jpeg => "jpg"u8,
            ImageFormat.Png => "png"u8,
            ImageFormat.WebP => "webp"u8,
            ImageFormat.Gif => "gif"u8,
            ImageFormat.Lottie => "json"u8,
            _ => throw new System.ComponentModel.InvalidEnumArgumentException("Invalid image format.")
        };
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a custom emoji.
    /// </summary>
    /// <param name="emojiId">The ID of the emoji.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the custom emoji.</returns>
    public static ImageUrl CustomEmoji(ulong emojiId, ImageFormat format)
    {
        return new($"/emojis/{emojiId}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild's icon.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="iconHash">The guild's icon hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated icons).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's icon.</returns>
    public static ImageUrl GuildIcon(ulong guildId, string iconHash, ImageFormat? format)
    {
        return new($"/icons/{guildId}/{iconHash}", GetExtension(iconHash, format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild's splash.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="splashHash">The guild's splash hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's splash.</returns>
    public static ImageUrl GuildSplash(ulong guildId, string splashHash, ImageFormat format)
    {
        return new($"/splashes/{guildId}/{splashHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild's discovery splash.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="discoverySplashHash">The guild's discovery splash hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's discovery splash.</returns>
    public static ImageUrl GuildDiscoverySplash(ulong guildId, string discoverySplashHash, ImageFormat format)
    {
        return new($"/discovery-splashes/{guildId}/{discoverySplashHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild's banner.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="bannerHash">The guild's banner hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated banners).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's banner.</returns>
    public static ImageUrl GuildBanner(ulong guildId, string bannerHash, ImageFormat? format)
    {
        return new($"/banners/{guildId}/{bannerHash}", GetExtension(bannerHash, format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a user's banner.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="bannerHash">The user's banner hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated banners).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the user's banner.</returns>
    public static ImageUrl UserBanner(ulong userId, string bannerHash, ImageFormat? format)
    {
        return new($"/banners/{userId}/{bannerHash}", GetExtension(bannerHash, format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the default user avatar for users on the legacy username system.
    /// </summary>
    /// <param name="discriminator">The user's discriminator.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the default user avatar.</returns>
    /// <remarks>
    /// This <see cref="ImageUrl"/> does not support setting size.
    /// </remarks>
    public static ImageUrl DefaultUserAvatar(ushort discriminator)
    {
        return new($"/embed/avatars/{discriminator % 5}", "png", supportsSize: false);
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the default user avatar for users on the new username system.
    /// </summary>
    /// <param name="id">The user's ID.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the default user avatar.</returns>
    /// <remarks>
    /// This <see cref="ImageUrl"/> does not support setting size.
    /// </remarks>
    public static ImageUrl DefaultUserAvatar(ulong id)
    {
        return new($"/embed/avatars/{(id >> 22) % 6}", "png", supportsSize: false);
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a user's avatar.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="avatarHash">The user's avatar hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated avatars).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the user's avatar.</returns>
    public static ImageUrl UserAvatar(ulong userId, string avatarHash, ImageFormat? format)
    {
        return new($"/avatars/{userId}/{avatarHash}", GetExtension(avatarHash, format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild user's avatar.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="avatarHash">The user's avatar hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated avatars).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild user's avatar.</returns>
    public static ImageUrl GuildUserAvatar(ulong guildId, ulong userId, string avatarHash, ImageFormat? format)
    {
        return new($"/guilds/{guildId}/users/{userId}/avatars/{avatarHash}", GetExtension(avatarHash, format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of an avatar decoration.
    /// </summary>
    /// <param name="avatarDecorationHash">The avatar decoration data asset hash.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the avatar decoration.</returns>
    public static ImageUrl AvatarDecoration(string avatarDecorationHash)
    {
        return new($"/avatar-decoration-presets/{avatarDecorationHash}", "png");
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of an application's icon.
    /// </summary>
    /// <param name="applicationId">The ID of the application.</param>
    /// <param name="iconHash">The application's icon hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the application's icon.</returns>
    public static ImageUrl ApplicationIcon(ulong applicationId, string iconHash, ImageFormat format)
    {
        return new($"/app-icons/{applicationId}/{iconHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of an application's cover image.
    /// </summary>
    /// <param name="applicationId">The ID of the application.</param>
    /// <param name="coverHash">The application's cover image hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the application's cover image.</returns>
    public static ImageUrl ApplicationCover(ulong applicationId, string coverHash, ImageFormat format)
    {
        return new($"/app-icons/{applicationId}/{coverHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of an application asset.
    /// </summary>
    /// <param name="applicationId">The ID of the application.</param>
    /// <param name="assetId">The ID of the asset.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the application asset.</returns>
    public static ImageUrl ApplicationAsset(ulong applicationId, ulong assetId, ImageFormat format)
    {
        return new($"/app-assets/{applicationId}/{assetId}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of an achievement icon.
    /// </summary>
    /// <param name="applicationId">The ID of the application.</param>
    /// <param name="achievementId">The ID of the achievement.</param>
    /// <param name="iconHash">The achievement's icon hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the achievement icon.</returns>
    public static ImageUrl AchievementIcon(ulong applicationId, ulong achievementId, string iconHash, ImageFormat format)
    {
        return new($"/app-assets/{applicationId}/achievements/{achievementId}/icons/{iconHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a store page asset.
    /// </summary>
    /// <param name="applicationId">The ID of the application.</param>
    /// <param name="assetId">The ID of the asset.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the store page asset.</returns>
    public static ImageUrl StorePageAsset(ulong applicationId, ulong assetId, ImageFormat format)
    {
        return new($"/app-assets/{applicationId}/store/{assetId}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a sticker pack banner.
    /// </summary>
    /// <param name="stickerPackBannerAssetId">The ID of the sticker pack banner asset.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the sticker pack banner.</returns>
    public static ImageUrl StickerPackBanner(ulong stickerPackBannerAssetId, ImageFormat format)
    {
        return new($"/app-assets/710982414301790216/store/{stickerPackBannerAssetId}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a team's icon.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="iconHash">The team's icon hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the team's icon.</returns>
    public static ImageUrl TeamIcon(ulong teamId, string iconHash, ImageFormat format)
    {
        return new($"/team-icons/{teamId}/{iconHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a sticker.
    /// </summary>
    /// <param name="stickerId">The ID of the sticker.</param>
    /// <param name="stickerFormat">The format type of the sticker.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the sticker.</returns>
    /// <remarks>
    /// This <see cref="ImageUrl"/> does not support setting size. Sticker GIFs use the media URL instead of the CDN URL.
    /// </remarks>
    public static ImageUrl Sticker(ulong stickerId, StickerFormat stickerFormat, ImageFormat format)
    {
        return new($"/stickers/{stickerId}", GetFormat(format), stickerFormat is StickerFormat.Gif ? Discord.MediaUrl : Discord.CDNUrl, false);
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a role's icon.
    /// </summary>
    /// <param name="roleId">The ID of the role.</param>
    /// <param name="iconHash">The role's icon hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the role's icon.</returns>
    public static ImageUrl RoleIcon(ulong roleId, string iconHash, ImageFormat format)
    {
        return new($"/role-icons/{roleId}/{iconHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild scheduled event's cover image.
    /// </summary>
    /// <param name="scheduledEventId">The ID of the scheduled event.</param>
    /// <param name="coverImageHash">The scheduled event's cover image hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild scheduled event's cover image.</returns>
    public static ImageUrl GuildScheduledEventCover(ulong scheduledEventId, string coverImageHash, ImageFormat format)
    {
        return new($"/guild-events/{scheduledEventId}/{coverImageHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild user's banner.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="bannerHash">The user's banner hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated banners).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild user's banner.</returns>
    public static ImageUrl GuildUserBanner(ulong guildId, ulong userId, string bannerHash, ImageFormat? format)
    {
        return new($"/guilds/{guildId}/users/{userId}/banners/{bannerHash}", GetExtension(bannerHash, format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild tag badge.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="badgeHash">The guild tag badge hash.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild tag badge.</returns>
    public static ImageUrl GuildTagBadge(ulong guildId, string badgeHash, ImageFormat format)
    {
        return new($"/guild-tag-badges/{guildId}/{badgeHash}", GetFormat(format));
    }

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a guild widget.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="style">The style of the widget. If <see langword="null"/>, uses the default style.</param>
    /// <param name="hostname">The hostname to use for the API request.</param>
    /// <param name="version">The API version to use.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild widget.</returns>
    /// <remarks>
    /// This <see cref="ImageUrl"/> does not support setting size.
    /// </remarks>
    public static ImageUrl GuildWidget(ulong guildId, GuildWidgetStyle? style = null, string hostname = Discord.RestHostname, ApiVersion? version = null)
    {
        return new(GetUrl(guildId, style, hostname, version), false);

        static string GetUrl(ulong guildId, GuildWidgetStyle? style, string hostname, ApiVersion? version)
        {
            return version.HasValue
                ? style.HasValue
                    ? $"https://{hostname}/api/v{(int)version.GetValueOrDefault()}/guilds/{guildId}/widget.png?style={GetGuildWidgetStyle(style.GetValueOrDefault())}"
                    : $"https://{hostname}/api/v{(int)version.GetValueOrDefault()}/guilds/{guildId}/widget.png"
                : style.HasValue
                    ? $"https://{hostname}/api/guilds/{guildId}/widget.png?style={GetGuildWidgetStyle(style.GetValueOrDefault())}"
                    : $"https://{hostname}/api/guilds/{guildId}/widget.png";
        }

        static string GetGuildWidgetStyle(GuildWidgetStyle style)
        {
            return style switch
            {
                GuildWidgetStyle.Shield => "shield",
                GuildWidgetStyle.Banner1 => "banner1",
                GuildWidgetStyle.Banner2 => "banner2",
                GuildWidgetStyle.Banner3 => "banner3",
                GuildWidgetStyle.Banner4 => "banner4",
                _ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(style), (int)style, typeof(GuildWidgetStyle)),
            };
        }
    }
}
