using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents event arguments for a voice channel effect send event.
/// </summary>
public class VoiceChannelEffectSendEventArgs(JsonVoiceChannelEffectSendEventArgs jsonModel, RestClient client) : IJsonModel<JsonVoiceChannelEffectSendEventArgs>
{
    JsonVoiceChannelEffectSendEventArgs IJsonModel<JsonVoiceChannelEffectSendEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the voice channel the effect was sent in.
    /// </summary>
    public ulong ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// The ID of the guild the effect was sent in.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The ID of the user who sent the effect.
    /// </summary>
    public ulong UserId => jsonModel.UserId;

    /// <summary>
    /// The emoji sent with the effect.
    /// </summary>
    public Emoji? Emoji { get; } = jsonModel.Emoji is { } emoji ? Emoji.CreateFromJson(emoji, jsonModel.GuildId, client) : null;

    /// <summary>
    /// The type of animation for the effect.
    /// </summary>
    public VoiceChannelEffectSendAnimationType? AnimationType => jsonModel.AnimationType;

    /// <summary>
    /// The ID of the animation for the effect.
    /// </summary>
    public ulong? AnimationId => jsonModel.AnimationId;

    /// <summary>
    /// The ID of the soundboard sound played with the effect.
    /// </summary>
    public ulong? SoundId => jsonModel.SoundId;

    /// <summary>
    /// The volume of the sound associated with the effect (from 0 to 1).
    /// </summary>
    public double? SoundVolume => jsonModel.SoundVolume;
}

/// <summary>
/// Represents the animation type of a voice channel effect.
/// </summary>
public enum VoiceChannelEffectSendAnimationType : byte
{
    /// <summary>
    /// A premium animation.
    /// </summary>
    Premium = 0,

    /// <summary>
    /// A basic animation.
    /// </summary>
    Basic = 1,
}
