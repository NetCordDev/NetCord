using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class VoiceChannelEffectSendEventArgs(JsonVoiceChannelEffectSendEventArgs jsonModel, RestClient client)
{
    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong GuildId { get; } = jsonModel.GuildId;

    public ulong UserId { get; } = jsonModel.UserId;

    public Emoji? Emoji { get; } = jsonModel.Emoji is { } emoji ? Emoji.Create(emoji, jsonModel.GuildId, client) : null;

    public VoiceChannelEffectSendAnimationType? AnimationType { get; } = jsonModel.AnimationType;

    public ulong? AnimationId { get; } = jsonModel.AnimationId;

    public ulong? SoundId { get; } = jsonModel.SoundId;

    public double? SoundVolume { get; } = jsonModel.SoundVolume;
}

public enum VoiceChannelEffectSendAnimationType : byte
{
    Premium = 0,
    Basic = 1,
}
