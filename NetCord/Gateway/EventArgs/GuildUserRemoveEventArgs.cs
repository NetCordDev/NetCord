using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildUserRemoveEventArgs(JsonModels.EventArgs.JsonGuildUserRemoveEventArgs jsonModel, RestClient client)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public User User { get; } = new(jsonModel.User, client);
}
