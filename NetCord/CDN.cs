namespace NetCord;

public static class CDN
{
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <param name="guildAvatarHash"></param>
    /// <param name="format"></param>
    /// <param name="size">Any power of 2 between 16 and 4096.</param>
    public static string GetGuildAvatarUrl(DiscordId guildId, DiscordId userId, string guildAvatarHash, ImageFormat? format = null, int? size = null)
    {
        if (guildAvatarHash != null)
        {
            return size.HasValue
                ? $"{Discord.ImageBaseUrl}/guilds/{guildId}/users/{userId}/avatars/{guildAvatarHash}.{(format.HasValue ? ImageProperties.GetFormat(format.GetValueOrDefault()) : guildAvatarHash.StartsWith("a_") ? "gif" : "png")}?size={size}"
                : $"{Discord.ImageBaseUrl}/guilds/{guildId}/users/{userId}/avatars/{guildAvatarHash}.{(format.HasValue ? ImageProperties.GetFormat(format.GetValueOrDefault()) : guildAvatarHash.StartsWith("a_") ? "gif" : "png")}";
        }
        else
            throw new InvalidOperationException("This user has no guild avatar");
    }

    /// <param name="userId"></param>
    /// <param name="avatarHash"></param>
    /// <param name="format"></param>
    /// <param name="size">Any power of 2 between 16 and 4096.</param>
    public static string GetAvatarUrl(DiscordId userId, string avatarHash, ImageFormat? format = null, int? size = null)
    {
        if (avatarHash != null)
        {
            return size.HasValue
                ? $"{Discord.ImageBaseUrl}/avatars/{userId}/{avatarHash}.{(format.HasValue ? ImageProperties.GetFormat(format.GetValueOrDefault()) : avatarHash.StartsWith("a_") ? "gif" : "png")}?size={size}"
                : $"{Discord.ImageBaseUrl}/avatars/{userId}/{avatarHash}.{(format.HasValue ? ImageProperties.GetFormat(format.GetValueOrDefault()) : avatarHash.StartsWith("a_") ? "gif" : "png")}";
        }
        else
            throw new InvalidOperationException("This user has no avatar");
    }

    public static string GetDefaultAvatarUrl(ushort discriminator) => $"{Discord.ImageBaseUrl}/embed/avatars/{discriminator % 5}.png";

    public static string GetGuildWidgetImageUrl(DiscordId guildId, GuildWidgetImageStyle? style = null)
    {
        if (style.HasValue)
            return $"https://discord.com/api/guilds/{guildId}/widget.png?style={(style.GetValueOrDefault() == GuildWidgetImageStyle.Shield ? "shield" : $"banner{(int)style.GetValueOrDefault()}")}";
        else
            return $"https://discord.com/api/guilds/{guildId}/widget.png";
    }
}