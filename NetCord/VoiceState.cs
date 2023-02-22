using NetCord.Rest;

namespace NetCord;

public class VoiceState : IJsonModel<JsonModels.JsonVoiceState>
{
    JsonModels.JsonVoiceState IJsonModel<JsonModels.JsonVoiceState>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonVoiceState _jsonModel;

    public VoiceState(JsonModels.JsonVoiceState jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (_jsonModel.User != null)
            User = new(_jsonModel.User, _jsonModel.GuildId.GetValueOrDefault(), client);
    }

    public ulong? GuildId => _jsonModel.GuildId;

    public ulong? ChannelId => _jsonModel.ChannelId;

    public ulong UserId => _jsonModel.UserId;

    public GuildUser? User { get; }

    public string SessionId => _jsonModel.SessionId;

    public bool IsDeafened => _jsonModel.IsDeafened;

    public bool IsMuted => _jsonModel.IsMuted;

    public bool IsSelfDeafened => _jsonModel.IsSelfDeafened;

    public bool IsSelfMuted => _jsonModel.IsSelfMuted;

    public bool? SelfStreamExists => _jsonModel.SelfStreamExists;

    public bool SelfVideoExists => _jsonModel.SelfVideoExists;

    public bool Suppressed => _jsonModel.Suppressed;

    public DateTimeOffset? RequestToSpeakTimestamp => _jsonModel.RequestToSpeakTimestamp;
}
