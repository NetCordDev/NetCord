using NetCord.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class GuildScheduledEventUserEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs>
{
    JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs _jsonModel;

    public GuildScheduledEventUserEventArgs(JsonGuildScheduledEventUserEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake GuildScheduledEventId => _jsonModel.GuildScheduledEventId;

    public Snowflake UserId => _jsonModel.UserId;

    public Snowflake GuildId => _jsonModel.GuildId;
}
