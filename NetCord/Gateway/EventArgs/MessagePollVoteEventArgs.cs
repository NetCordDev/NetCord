namespace NetCord.Gateway;

public class MessagePollVoteEventArgs(JsonModels.EventArgs.JsonMessagePollVoteEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonMessagePollVoteEventArgs>
{
    JsonModels.EventArgs.JsonMessagePollVoteEventArgs IJsonModel<JsonModels.EventArgs.JsonMessagePollVoteEventArgs>.JsonModel => jsonModel;

    public ulong UserId => jsonModel.UserId;
    
    public ulong ChannelId => jsonModel.ChannelId;
    
    public ulong MessageId => jsonModel.MessageId;
    
    public ulong? GuildId => jsonModel.GuildId;
    
    public int AnswerId => jsonModel.AnswerId;
}
