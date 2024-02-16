namespace NetCord.Gateway;

public class MessageDeleteEventArgs(JsonModels.EventArgs.JsonMessageDeleteEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonMessageDeleteEventArgs>
{
    JsonModels.EventArgs.JsonMessageDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageDeleteEventArgs>.JsonModel => jsonModel;

    public ulong MessageId => jsonModel.MessageId;

    public ulong ChannelId => jsonModel.ChannelId;

    public ulong? GuildId => jsonModel.GuildId;
}
