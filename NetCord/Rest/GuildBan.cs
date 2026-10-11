namespace NetCord.Rest;

public partial class GuildBan(JsonModels.JsonGuildBan jsonModel, ulong guildId, RestClient client)
{
    public string? Reason { get; } = jsonModel.Reason;

    public User User { get; } = new(jsonModel.User, client);

    public ulong GuildId { get; } = guildId;
}
