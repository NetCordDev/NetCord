namespace NetCord.Rest;

public class GuildWidgetSettings : IJsonModel<JsonModels.JsonGuildWidgetSettings>
{
    JsonModels.JsonGuildWidgetSettings IJsonModel<JsonModels.JsonGuildWidgetSettings>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildWidgetSettings _jsonModel;

    public bool Enabled => _jsonModel.Enabled;

    public Snowflake? ChannelId => _jsonModel.ChannelId;

    public GuildWidgetSettings(JsonModels.JsonGuildWidgetSettings jsonModel)
    {
        _jsonModel = jsonModel;
    }
}