using NetCord.Rest;

namespace NetCord.Gateway;

public class VoiceState(JsonModels.JsonVoiceState jsonModel, ulong guildId, RestClient client)
{
    public ulong GuildId { get; } = guildId;

    public ulong? ChannelId { get; } = jsonModel.ChannelId;

    public ulong UserId { get; } = jsonModel.UserId;

    public GuildUser? User { get; } = jsonModel.User is { } user ? new(user, jsonModel.GuildId.GetValueOrDefault(), client) : null;

    public string SessionId { get; } = jsonModel.SessionId;

    public bool Deaf { get; } = jsonModel.Deaf;

    public bool Mute { get; } = jsonModel.Mute;

    public bool SelfDeaf { get; } = jsonModel.SelfDeaf;

    public bool SelfMute { get; } = jsonModel.SelfMute;

    public bool? SelfStream { get; } = jsonModel.SelfStream;

    public bool SelfVideo { get; } = jsonModel.SelfVideo;

    public bool Suppress { get; } = jsonModel.Suppress;

    public DateTimeOffset? RequestToSpeakTimestamp { get; } = jsonModel.RequestToSpeakTimestamp;
}
