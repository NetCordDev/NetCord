using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class GuildUser(JsonGuildUser jsonModel, ulong guildId, RestClient client) : PartialGuildUser(jsonModel, client)
{
    public ulong GuildId { get; } = guildId;

    public ImageUrl GetGuildAvatarUrl(ImageFormat? format = null) => ImageUrl.GuildUserAvatar(GuildId, Id, GuildAvatarHash!, format);

    public Task<GuildUser> TimeOutAsync(DateTimeOffset until, RestRequestProperties? properties = null) => ModifyAsync(u => u.TimeOutUntil = until, properties);

    public async Task<GuildUserInfo> GetInfoAsync(RestRequestProperties? properties = null)
    {
        await foreach (var info in _client.SearchGuildUsersAsync(GuildId, new() { Limit = 1, AndQuery = [new UserIdsGuildUsersSearchQuery([Id])] }, properties).ConfigureAwait(false))
            return info;

        throw new EntityNotFoundException("The user was not found.");
    }
}
