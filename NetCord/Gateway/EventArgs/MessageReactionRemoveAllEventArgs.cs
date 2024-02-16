namespace NetCord.Gateway;

public class MessageReactionRemoveAllEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs>.JsonModel => jsonModel;

    public ulong ChannelId => jsonModel.ChannelId;

    public ulong MessageId => jsonModel.MessageId;

    public ulong? GuildId => jsonModel.GuildId;
}
