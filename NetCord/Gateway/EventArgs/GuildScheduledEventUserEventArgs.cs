namespace NetCord.Gateway;

public class GuildScheduledEventUserEventArgs(JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs>
{
    JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs>.JsonModel => jsonModel;

    public ulong GuildScheduledEventId => jsonModel.GuildScheduledEventId;

    public ulong UserId => jsonModel.UserId;

    public ulong GuildId => jsonModel.GuildId;
}
