using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NetCord;

#pragma warning disable IDE0032 // Use auto property

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

    public bool SupportsSize => _supportsSize;

    public override string ToString() => _url;

    public virtual string ToString(int size)
    {
        if (!_supportsSize)
            ThrowSizeNotSupported();

        return $"{_url}?size={size}";
    }

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

    public static ImageUrl CustomEmoji(ulong emojiId, ImageFormat format)
    {
        return new($"/emojis/{emojiId}", GetFormat(format));
    }

    public static ImageUrl GuildIcon(ulong guildId, string iconHash, ImageFormat? format)
    {
        return new($"/icons/{guildId}/{iconHash}", GetExtension(iconHash, format));
    }

    public static ImageUrl GuildSplash(ulong guildId, string splashHash, ImageFormat format)
    {
        return new($"/splashes/{guildId}/{splashHash}", GetFormat(format));
    }

    public static ImageUrl GuildDiscoverySplash(ulong guildId, string discoverySplashHash, ImageFormat format)
    {
        return new($"/discovery-splashes/{guildId}/{discoverySplashHash}", GetFormat(format));
    }

    public static ImageUrl GuildBanner(ulong guildId, string bannerHash, ImageFormat? format)
    {
        return new($"/banners/{guildId}/{bannerHash}", GetExtension(bannerHash, format));
    }

    public static ImageUrl UserBanner(ulong userId, string bannerHash, ImageFormat? format)
    {
        return new($"/banners/{userId}/{bannerHash}", GetExtension(bannerHash, format));
    }

    public static ImageUrl DefaultUserAvatar(ushort discriminator)
    {
        return new($"/embed/avatars/{discriminator % 5}", "png", supportsSize: false);
    }

    public static ImageUrl DefaultUserAvatar(ulong id)
    {
        return new($"/embed/avatars/{(id >> 22) % 6}", "png", supportsSize: false);
    }

    public static ImageUrl UserAvatar(ulong userId, string avatarHash, ImageFormat? format)
    {
        return new($"/avatars/{userId}/{avatarHash}", GetExtension(avatarHash, format));
    }

    public static ImageUrl GuildUserAvatar(ulong guildId, ulong userId, string avatarHash, ImageFormat? format)
    {
        return new($"/guilds/{guildId}/users/{userId}/avatars/{avatarHash}", GetExtension(avatarHash, format));
    }

    public static ImageUrl AvatarDecoration(string avatarDecorationHash)
    {
        return new($"/avatar-decoration-presets/{avatarDecorationHash}", "png");
    }

    public static ImageUrl ApplicationIcon(ulong applicationId, string iconHash, ImageFormat format)
    {
        return new($"/app-icons/{applicationId}/{iconHash}", GetFormat(format));
    }

    public static ImageUrl ApplicationCover(ulong applicationId, string coverHash, ImageFormat format)
    {
        return new($"/app-icons/{applicationId}/{coverHash}", GetFormat(format));
    }

    public static ImageUrl ApplicationAsset(ulong applicationId, ulong assetId, ImageFormat format)
    {
        return new($"/app-assets/{applicationId}/{assetId}", GetFormat(format));
    }

    public static ImageUrl AchievementIcon(ulong applicationId, ulong achievementId, string iconHash, ImageFormat format)
    {
        return new($"/app-assets/{applicationId}/achievements/{achievementId}/icons/{iconHash}", GetFormat(format));
    }

    public static ImageUrl StorePageAsset(ulong applicationId, ulong assetId, ImageFormat format)
    {
        return new($"/app-assets/{applicationId}/store/{assetId}", GetFormat(format));
    }

    public static ImageUrl StickerPackBanner(ulong stickerPackBannerAssetId, ImageFormat format)
    {
        return new($"/app-assets/710982414301790216/store/{stickerPackBannerAssetId}", GetFormat(format));
    }

    public static ImageUrl TeamIcon(ulong teamId, string iconHash, ImageFormat format)
    {
        return new($"/team-icons/{teamId}/{iconHash}", GetFormat(format));
    }

    public static ImageUrl Sticker(ulong stickerId, StickerFormat stickerFormat, ImageFormat format)
    {
        return new($"/stickers/{stickerId}", GetFormat(format), stickerFormat is StickerFormat.Gif ? Discord.MediaUrl : Discord.CDNUrl, false);
    }

    public static ImageUrl RoleIcon(ulong roleId, string iconHash, ImageFormat format)
    {
        return new($"/role-icons/{roleId}/{iconHash}", GetFormat(format));
    }

    public static ImageUrl GuildScheduledEventCover(ulong scheduledEventId, string coverImageHash, ImageFormat format)
    {
        return new($"/guild-events/{scheduledEventId}/{coverImageHash}", GetFormat(format));
    }

    public static ImageUrl GuildUserBanner(ulong guildId, ulong userId, string bannerHash, ImageFormat? format)
    {
        return new($"/guilds/{guildId}/users/{userId}/banners/{bannerHash}", GetExtension(bannerHash, format));
    }

    public static ImageUrl GuildTagBadge(ulong guildId, string badgeHash, ImageFormat format)
    {
        return new($"/guild-tag-badges/{guildId}/{badgeHash}", GetFormat(format));
    }

    public static ImageUrl GuildWidget(ulong guildId, GuildWidgetStyle? style = null, string? hostname = null, ApiVersion? version = null)
    {
        return new(GetUrl(guildId, style, hostname, version), false);

        static string GetUrl(ulong guildId, GuildWidgetStyle? style, string? hostname, ApiVersion? version)
        {
            return version.HasValue
                ? style.HasValue
                    ? $"https://{hostname ?? Discord.RestHostname}/api/v{(int)version.GetValueOrDefault()}/guilds/{guildId}/widget.png?style={GetGuildWidgetStyle(style.GetValueOrDefault())}"
                    : $"https://{hostname ?? Discord.RestHostname}/api/v{(int)version.GetValueOrDefault()}/guilds/{guildId}/widget.png"
                : style.HasValue
                    ? $"https://{hostname ?? Discord.RestHostname}/api/guilds/{guildId}/widget.png?style={GetGuildWidgetStyle(style.GetValueOrDefault())}"
                    : $"https://{hostname ?? Discord.RestHostname}/api/guilds/{guildId}/widget.png";
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
