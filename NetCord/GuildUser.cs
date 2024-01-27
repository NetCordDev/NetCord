using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class GuildUser : PartialGuildUser
{
    public ulong GuildId { get; }

    public GuildUser(JsonGuildUser jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
    }

    public ImageUrl GetGuildAvatarUrl(ImageFormat? format = null) => ImageUrl.GuildUserAvatar(GuildId, Id, GuildAvatarHash!, format);

    public Task<GuildUser> TimeOutAsync(DateTimeOffset until, RequestProperties? properties = null) => ModifyAsync(u => u.TimeOutUntil = until, properties);

    public async Task<GuildUserInfo> GetInfoAsync(RequestProperties? properties = null)
    {
        await foreach (var info in _client.SearchGuildUsersAsync(GuildId, new() { Limit = 1, AndQuery = [new UserIdsGuildUsersSearchQuery([Id])] }, properties).ConfigureAwait(false))
            return info;

        throw new EntityNotFoundException("The user was not found.");
    }
}
