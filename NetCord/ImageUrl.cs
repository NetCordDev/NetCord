namespace NetCord;

public class ImageUrl
{
    private string _url;

    private ImageUrl(string partialUrl, string extension)
    {
        _url = $"{Discord.CDNUrl}{partialUrl}.{extension}";
    }

    public override string ToString() => _url;

    public string ToString(int size) => $"{_url}?size={size}";

    private static string GetExtension(string hash, ImageFormat? format)
    {
        return format.HasValue
            ? GetFormat(format.GetValueOrDefault())
            : hash.StartsWith("a_") ? "gif" : "png";
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

    public static ImageUrl UserAvatar(ulong userId, string avatarHash, ImageFormat? format)
    {
        return new($"/avatars/{userId}/{avatarHash}", GetExtension(avatarHash, format));
    }

    public static ImageUrl GuildUserAvatar(ulong guildId, ulong userId, string avatarHash, ImageFormat? format)
    {
        return new($"/guilds/{guildId}/users/{userId}/avatars/{avatarHash}", GetExtension(avatarHash, format));
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

    public static ImageUrl GuildRoleIcon(ulong roleId, ImageFormat format)
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

    public static string GuildWidget(ulong guildId, GuildWidgetStyle? style = null)
    {
        if (!style.HasValue)
            return $"{Discord.RestUrl}/guilds/{guildId}/widget.png";
        else
            return $"{Discord.RestUrl}/guilds/{guildId}/widget.png?style={style switch
            {
                GuildWidgetStyle.Shield => "shield",
                GuildWidgetStyle.Banner1 => "banner1",
                GuildWidgetStyle.Banner2 => "banner2",
                GuildWidgetStyle.Banner3 => "banner3",
                GuildWidgetStyle.Banner4 => "banner4",
                _ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(style), (int)style, typeof(GuildWidgetStyle)),
            }}";
    }
}
