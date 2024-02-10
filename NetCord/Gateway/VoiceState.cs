using NetCord.Rest;

namespace NetCord.Gateway;

public class VoiceState : IJsonModel<JsonModels.JsonVoiceState>
{
    JsonModels.JsonVoiceState IJsonModel<JsonModels.JsonVoiceState>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonVoiceState _jsonModel;

    public VoiceState(JsonModels.JsonVoiceState jsonModel, ulong guildId, RestClient client)
    {
        _jsonModel = jsonModel;

        GuildId = guildId;

        var user = jsonModel.User;
        if (user is not null)
            User = new(user, _jsonModel.GuildId.GetValueOrDefault(), client);
    }

    public ulong GuildId { get; }

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
