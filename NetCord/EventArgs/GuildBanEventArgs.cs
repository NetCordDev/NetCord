using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildBanEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildBanEventArgs>
{
    JsonModels.EventArgs.JsonGuildBanEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildBanEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildBanEventArgs _jsonModel;

    public GuildBanEventArgs(JsonGuildBanEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(_jsonModel.User, client);
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public User User { get; }
}
