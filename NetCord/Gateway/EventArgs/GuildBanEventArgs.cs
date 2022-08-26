using NetCord.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildBanEventArgs : IJsonModel<JsonGuildBanEventArgs>
{
    JsonGuildBanEventArgs IJsonModel<JsonGuildBanEventArgs>.JsonModel => _jsonModel;
    private readonly JsonGuildBanEventArgs _jsonModel;

    public GuildBanEventArgs(JsonGuildBanEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(_jsonModel.User, client);
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public User User { get; }
}
