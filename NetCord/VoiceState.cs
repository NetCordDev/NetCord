namespace NetCord;

public class VoiceState
{
    internal JsonModels.JsonVoiceState _jsonModel;

    public ulong? GuildId => _jsonModel.GuildId;

    public ulong? ChannelId => _jsonModel.ChannelId;

    public ulong UserId => _jsonModel.UserId;

    //public GuildUser? User => _jsonModel.User;

    public string SessionId => _jsonModel.SessionId;

    public bool IsDeafened => _jsonModel.IsDeafened;

    public bool IsMuted => _jsonModel.IsMuted;

    public bool IsSelfDeafened => _jsonModel.IsSelfDeafened;

    public bool IsSelfMuted => _jsonModel.IsSelfMuted;

    public bool? SelfStreamExists => _jsonModel.SelfStreamExists;

    public bool SelfVideoExists => _jsonModel.SelfVideoExists;

    public bool Suppressed => _jsonModel.Suppressed;

    public DateTimeOffset? RequestToSpeakTimestamp => _jsonModel.RequestToSpeakTimestamp;

    public VoiceState(JsonModels.JsonVoiceState jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
