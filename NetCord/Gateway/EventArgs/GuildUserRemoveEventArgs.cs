using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildUserRemoveEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildUserRemoveEventArgs>
{
    JsonModels.EventArgs.JsonGuildUserRemoveEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildUserRemoveEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildUserRemoveEventArgs _jsonModel;

    public GuildUserRemoveEventArgs(JsonModels.EventArgs.JsonGuildUserRemoveEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(jsonModel.User, client);
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public User User { get; }
}
