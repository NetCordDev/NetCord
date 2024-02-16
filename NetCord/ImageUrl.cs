namespace NetCord;

public class ImageUrl : ISpanFormattable
{
    private readonly string _url;

    private ImageUrl(string partialUrl, string extension) : this($"{Discord.CDNUrl}{partialUrl}.{extension}")
    {
    }

    protected ImageUrl(string url)
    {
        _url = url;
    }

    public override string ToString() => _url;

    public virtual string ToString(int size) => $"{_url}?size={size}";

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
            return _url;

        if (IsSizetValid(format))
            return $"{_url}?size={format}";

        throw new FormatException("Format specifier was invalid.");
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

        if (IsSizetValid(format))
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

        throw new FormatException("Format specifier was invalid.");
    }

    private protected virtual bool IsSizetValid(ReadOnlySpan<char> format)
    {
        for (int i = 0; i < format.Length; i++)
        {
            if (!char.IsAsciiDigit(format[i]))
                return false;
        }
        return true;
    }

    private static string GetExtension(string hash, ImageFormat? format)
    {
        return format.HasValue
            ? GetFormat(format.GetValueOrDefault())
            : (hash.StartsWith("a_") ? "gif" : "png");
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
        return new($"/splashes/{guildId}/{splashHash}/guild_splash", GetFormat(format));
    }

    public static ImageUrl GuildDiscoverySplash(ulong guildId, string discoverySplashHash, ImageFormat format)
    {
        return new($"/discovery-splashes/{guildId}/{discoverySplashHash}/guild_splash", GetFormat(format));
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
        return new($"/embed/avatars/{discriminator % 5}", "png");
    }

    public static ImageUrl DefaultUserAvatar(ulong id)
    {
        return new($"/embed/avatars/{(id >> 22) % 6}", "png");
    }

    public static ImageUrl UserAvatar(ulong userId, string avatarHash, ImageFormat? format)
    {
        return new($"/avatars/{userId}/{avatarHash}", GetExtension(avatarHash, format));
    }

    public static ImageUrl GuildUserAvatar(ulong guildId, ulong userId, string avatarHash, ImageFormat? format)
    {
        return new($"/guilds/{guildId}/users/{userId}/avatars/{avatarHash}", GetExtension(avatarHash, format));
    }

    public static ImageUrl UserAvatarDecoration(ulong userId, string avatarDecorationHash)
    {
        return new($"/avatar-decorations/{userId}/{avatarDecorationHash}", "png");
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
        return new($"/app-icons/{applicationId}/{assetId}", GetFormat(format));
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

    public static ImageUrl Sticker(ulong stickerId, ImageFormat format)
    {
        return new($"/stickers/{stickerId}", GetFormat(format));
    }

    public static ImageUrl RoleIcon(ulong roleId, ImageFormat format)
    {
        return new($"/role-icons/{roleId}/role_icon", GetFormat(format));
    }

    public static ImageUrl GuildScheduledEventCover(ulong scheduledEventId, string coverHash, ImageFormat format)
    {
        return new($"/guild-events/{scheduledEventId}/{coverHash}", GetFormat(format));
    }

    public static ImageUrl GuildUserBanner(ulong guildId, ulong userId, string bannerHash, ImageFormat? format)
    {
        return new($"/guilds/{guildId}/users/{userId}/banners/{bannerHash}", GetExtension(bannerHash, format));
    }

    public static ImageUrl GuildWidget(ulong guildId, GuildWidgetStyle? style = null, string? hostname = null, ApiVersion? version = null)
    {
        return new GuildWidgetUrl(guildId, style, hostname, version);
    }

    private class GuildWidgetUrl(ulong guildId, GuildWidgetStyle? style, string? hostname, ApiVersion? version) : ImageUrl(GetUrl(guildId, style, hostname, version))
    {
        private static string GetUrl(ulong guildId, GuildWidgetStyle? style, string? hostname, ApiVersion? version)
        {
            return version.HasValue
                ? style.HasValue
                    ? $"https://{hostname ?? Discord.RestHostname}/api/v{(int)version.GetValueOrDefault()}/guilds/{guildId}/widget.png?style={GetGuildWidgetStyle(style.GetValueOrDefault())}"
                    : $"https://{hostname ?? Discord.RestHostname}/api/v{(int)version.GetValueOrDefault()}/guilds/{guildId}/widget.png"
                : style.HasValue
                    ? $"https://{hostname ?? Discord.RestHostname}/api/guilds/{guildId}/widget.png?style={GetGuildWidgetStyle(style.GetValueOrDefault())}"
                    : $"https://{hostname ?? Discord.RestHostname}/api/guilds/{guildId}/widget.png";
        }

        private static string GetGuildWidgetStyle(GuildWidgetStyle style)
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

        public override string ToString(int size)
        {
            throw new NotSupportedException("Guild widgets do not support setting size.");
        }

        private protected override bool IsSizetValid(ReadOnlySpan<char> format) => false;
    }
}
