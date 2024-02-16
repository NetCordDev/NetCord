namespace NetCord.Rest;

public class GuildWidgetSettings(JsonModels.JsonGuildWidgetSettings jsonModel) : IJsonModel<JsonModels.JsonGuildWidgetSettings>
{
    JsonModels.JsonGuildWidgetSettings IJsonModel<JsonModels.JsonGuildWidgetSettings>.JsonModel => jsonModel;

    public bool Enabled => jsonModel.Enabled;

    public ulong? ChannelId => jsonModel.ChannelId;
}
