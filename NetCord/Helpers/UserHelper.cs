namespace NetCord;

public static class UserHelper
{
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

    public static async Task<DMChannel> GetDMChannelAsync(BotClient client, DiscordId userId)
        => new DMChannel((await CDN.SendAsync(HttpMethod.Post, $"{{\"recipient_id\":\"{userId}\"}}", "/users/@me/channels", client).ConfigureAwait(false)).ToObject<JsonModels.JsonChannel>(), client);
}