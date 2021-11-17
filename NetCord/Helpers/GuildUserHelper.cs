using System.Text.Json;

namespace NetCord;

public static class GuildUserHelper
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

    public static async Task<GuildUser> ModifyUserAsync(BotClient client, DiscordId guildId, DiscordId userId, Action<GuildUserProperties> func)
    {
        GuildUserProperties properties = new();
        func.Invoke(properties);
        var message = JsonSerializer.Serialize(properties);
        var result = await CDN.SendAsync(HttpMethod.Patch, message, $"/guilds/{guildId}/members/{userId}", client).ConfigureAwait(false);
        return new(result.ToObject<JsonModels.JsonGuildUser>(), client.GetGuild(guildId), client);
    }

    public static Task AddRoleAsync(BotClient client, DiscordId guildId, DiscordId userId, DiscordId roleId)
        => CDN.SendAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", client);

    public static Task AddRoleAsync(BotClient client, DiscordId guildId, DiscordId userId, DiscordId roleId, string reason)
        => CDN.SendAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", client, reason);

    public static Task RemoveRoleAsync(BotClient client, DiscordId guildId, DiscordId userId, DiscordId roleId)
        => CDN.SendAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", client);

    public static Task RemoveRoleAsync(BotClient client, DiscordId guildId, DiscordId userId, DiscordId roleId, string reason)
        => CDN.SendAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", client, reason);
}