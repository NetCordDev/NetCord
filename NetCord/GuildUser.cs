using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class GuildUser : PartialGuildUser
{
    public ulong GuildId { get; }

    public GuildUser(JsonGuildUser jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
    }

    public ImageUrl GetGuildAvatarUrl(ImageFormat? format = null) => ImageUrl.GuildUserAvatar(GuildId, Id, GuildAvatarHash!, format);

    public Task<GuildUser> TimeOutAsync(DateTimeOffset until, RequestProperties? properties = null) => ModifyAsync(u => u.TimeOutUntil = until, properties);

    #region Guild
    public Task<GuildUser> ModifyAsync(Action<GuildUserOptions> action, RequestProperties? properties = null) => _client.ModifyGuildUserAsync(GuildId, Id, action, properties);
    public Task AddRoleAsync(ulong roleId, RequestProperties? properties = null) => _client.AddGuildUserRoleAsync(GuildId, Id, roleId, properties);
    public Task RemoveRoleAsync(ulong roleId, RequestProperties? properties = null) => _client.RemoveGuildUserRoleAsync(GuildId, Id, roleId, properties);
    public Task KickAsync(RequestProperties? properties = null) => _client.KickGuildUserAsync(GuildId, Id, properties);
    public Task BanAsync(int deleteMessageSeconds = 0, RequestProperties? properties = null) => _client.BanGuildUserAsync(GuildId, Id, deleteMessageSeconds, properties);
    public Task UnbanAsync(RequestProperties? properties = null) => _client.UnbanGuildUserAsync(GuildId, Id, properties);
    public Task ModifyVoiceStateAsync(ulong channelId, Action<VoiceStateOptions> action, RequestProperties? properties = null) => _client.ModifyGuildUserVoiceStateAsync(GuildId, channelId, Id, action, properties);
    #endregion
}
