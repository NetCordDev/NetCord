namespace NetCord;

public class MessageReference(JsonModels.JsonMessageReference jsonModel) : IJsonModel<JsonModels.JsonMessageReference>
{
    JsonModels.JsonMessageReference IJsonModel<JsonModels.JsonMessageReference>.JsonModel => jsonModel;

    public ulong MessageId => jsonModel.MessageId.GetValueOrDefault();
    public ulong ChannelId => jsonModel.ChannelId.GetValueOrDefault();
    public ulong? GuildId => jsonModel.GuildId;
    public bool? FailIfNotExists => jsonModel.FailIfNotExists;
}
