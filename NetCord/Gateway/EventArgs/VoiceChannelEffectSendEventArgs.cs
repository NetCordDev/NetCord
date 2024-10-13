using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class VoiceChannelEffectSendEventArgs : IJsonModel<JsonVoiceChannelEffectSendEventArgs>
{
    JsonVoiceChannelEffectSendEventArgs IJsonModel<JsonVoiceChannelEffectSendEventArgs>.JsonModel => _jsonModel;
    private readonly JsonVoiceChannelEffectSendEventArgs _jsonModel;

    public VoiceChannelEffectSendEventArgs(JsonVoiceChannelEffectSendEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = Emoji.CreateFromJson(emoji, jsonModel.GuildId, client);
    }

    public ulong ChannelId => _jsonModel.ChannelId;

    public ulong GuildId => _jsonModel.GuildId;

    public ulong UserId => _jsonModel.UserId;

    public Emoji? Emoji { get; }

    public VoiceChannelEffectSendAnimationType? AnimationType => _jsonModel.AnimationType;

    public ulong? AnimationId => _jsonModel.AnimationId;

    public ulong? SoundId => _jsonModel.SoundId;

    public double? SoundVolume => _jsonModel.SoundVolume;
}

public enum VoiceChannelEffectSendAnimationType : byte
{
    Premium = 0,
    Basic = 1,
}
