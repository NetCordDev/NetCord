namespace NetCord;

public static class CDN
{
    public static string GetGuildAvatarUrl(DiscordId guildId, DiscordId userId, string guildAvatarHash, ImageFormat? format)
    {
        if (guildAvatarHash != null)
            return $"{Discord.ImageBaseUrl}/guilds/{guildId}/users/{userId}/avatars/{guildAvatarHash}.{(format.HasValue ? InternalHelper.GetImageExtension(format.GetValueOrDefault()) : guildAvatarHash.StartsWith("a_") ? "gif" : "png")}";
        else
            throw new InvalidOperationException("This user has no guild avatar");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <param name="guildAvatarHash"></param>
    /// <param name="format"></param>
    /// <param name="size">any power of two between 16 and 4096</param>
    /// <returns></returns>
    public static string GetGuildAvatarUrl(DiscordId guildId, DiscordId userId, string guildAvatarHash, ImageFormat? format, int size)
    {
        if (guildAvatarHash != null)
            return $"{Discord.ImageBaseUrl}/guilds/{guildId}/users/{userId}/avatars/{guildAvatarHash}.{(format.HasValue ? InternalHelper.GetImageExtension(format.GetValueOrDefault()) : guildAvatarHash.StartsWith("a_") ? "gif" : "png")}?size={size}";
        else
            throw new InvalidOperationException("This user has no guild avatar");
    }

    public static string GetAvatarUrl(DiscordId userId, string avatarHash, ImageFormat? format)
    {
        if (avatarHash != null)
            return $"{Discord.ImageBaseUrl}/avatars/{userId}/{avatarHash}.{(format.HasValue ? InternalHelper.GetImageExtension(format.GetValueOrDefault()) : avatarHash.StartsWith("a_") ? "gif" : "png")}";
        else
            throw new InvalidOperationException("This user has no avatar");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="avatarHash"></param>
    /// <param name="format"></param>
    /// <param name="size">any power of two between 16 and 4096</param>
    /// <returns></returns>
    public static string GetAvatarUrl(DiscordId userId, string avatarHash, int size, ImageFormat? format)
    {
        if (avatarHash != null)
            return $"{Discord.ImageBaseUrl}/avatars/{userId}/{avatarHash}.{(format.HasValue ? InternalHelper.GetImageExtension(format.GetValueOrDefault()) : avatarHash.StartsWith("a_") ? "gif" : "png")}?size={size}";
        else
            throw new InvalidOperationException("This user has no avatar");
    }

    public static string GetDefaultAvatarUrl(ushort discriminator) => $"{Discord.ImageBaseUrl}/embed/avatars/{discriminator % 5}.png";

    public static string GetGuildWidgetImageUrl(DiscordId guildId, GuildWidgetImageStyle? style = null)
    {
        if (style.HasValue)
            return $"https://discord.com/api/guilds/{guildId}/widget.png";
        else
            return $"https://discord.com/api/guilds/{guildId}/widget.png?style={(style.GetValueOrDefault() == GuildWidgetImageStyle.Shield ? "shield" : $"banner{style.GetValueOrDefault()}")}";
    }
}

public enum GuildWidgetImageStyle
{
    /// <summary>
    /// Example: <see href="https://discord.com/api/guilds/81384788765712384/widget.png?style=shield"/>
    /// </summary>
    Shield = 0,
    /// <summary>
    /// Example: <see href="https://discord.com/api/guilds/81384788765712384/widget.png?style=banner1"/>
    /// </summary>
    Banner1 = 1,
    /// <summary>
    /// Example: <see href="https://discord.com/api/guilds/81384788765712384/widget.png?style=banner2"/>
    /// </summary>
    Banner2 = 2,
    /// <summary>
    /// Example: <see href="https://discord.com/api/guilds/81384788765712384/widget.png?style=banner3"/>
    /// </summary>
    Banner3 = 3,
    /// <summary>
    /// Example: <see href="https://discord.com/api/guilds/81384788765712384/widget.png?style=banner4"/>
    /// </summary>
    Banner4 = 4,
}