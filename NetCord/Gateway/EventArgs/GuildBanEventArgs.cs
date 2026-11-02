using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildBanEventArgs(JsonModels.EventArgs.JsonGuildBanEventArgs jsonModel, RestClient client)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public User User { get; } = new(jsonModel.User, client);
}
