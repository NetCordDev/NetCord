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
}
