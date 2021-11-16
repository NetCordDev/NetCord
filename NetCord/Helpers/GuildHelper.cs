namespace NetCord;

public static class GuildHelper
{
    public static Task KickUserAsync(BotClient client, DiscordId userId, DiscordId guildId)
        => CDN.SendAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", client);

    public static Task KickUserAsync(BotClient client, DiscordId userId, DiscordId guildId, string reason)
        => CDN.SendAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", client, reason);

    public static Task AddBanAsync(BotClient client, DiscordId userId, DiscordId guildId)
        => CDN.SendAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", client);

    public static Task AddBanAsync(BotClient client, DiscordId userId, DiscordId guildId, string reason)
        => CDN.SendAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", client, reason);

    public static Task AddBanAsync(BotClient client, DiscordId userId, DiscordId guildId, int deleteMessageDays, string reason)
        => CDN.SendAsync(HttpMethod.Put, $"{{\"delete_message_days\":{deleteMessageDays}}}", $"/guilds/{guildId}/bans/{userId}", client, reason);

    public static Task RemoveBanAsync(BotClient client, DiscordId userId, DiscordId guildId)
        => CDN.SendAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", client);

    public static Task RemoveBanAsync(BotClient client, DiscordId userId, DiscordId guildId, string reason)
        => CDN.SendAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", client, reason);
}