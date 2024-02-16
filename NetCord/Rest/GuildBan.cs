namespace NetCord.Rest;

public partial class GuildBan(JsonModels.JsonGuildBan jsonModel, ulong guildId, RestClient client) : IJsonModel<JsonModels.JsonGuildBan>
{
    JsonModels.JsonGuildBan IJsonModel<JsonModels.JsonGuildBan>.JsonModel => jsonModel;

    public string? Reason => jsonModel.Reason;

    public User User { get; } = new(jsonModel.User, client);

    public ulong GuildId { get; } = guildId;
}
