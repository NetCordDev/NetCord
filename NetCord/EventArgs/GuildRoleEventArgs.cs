namespace NetCord;

public class GuildRoleEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildRoleEventArgs>
{
    JsonModels.EventArgs.JsonGuildRoleEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildRoleEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildRoleEventArgs _jsonModel;

    public GuildRoleEventArgs(JsonModels.EventArgs.JsonGuildRoleEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Role = new(jsonModel.Role, client);
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public GuildRole Role { get; }
}
