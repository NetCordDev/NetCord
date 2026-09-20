using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents a user's activity in a presence update.
/// </summary>
public class UserActivity(JsonModels.JsonUserActivity jsonModel, ulong guildId, RestClient client) : IJsonModel<JsonModels.JsonUserActivity>
{
    JsonModels.JsonUserActivity IJsonModel<JsonModels.JsonUserActivity>.JsonModel => jsonModel;

    /// <summary>
    /// The activity's name.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The activity type.
    /// </summary>
    public UserActivityType Type => jsonModel.Type;

    /// <summary>
    /// The stream URL, if the activity type is <see cref="UserActivityType.Streaming"/>.
    /// </summary>
    public string? Url => jsonModel.Url;

    /// <summary>
    /// When the activity was added to the user's session.
    /// </summary>
    public DateTimeOffset CreatedAt => jsonModel.CreatedAt;

    /// <summary>
    /// Timestamps for start and/or end of the game.
    /// </summary>
    public UserActivityTimestamps? Timestamps { get; } = jsonModel.Timestamps is { } timestamps ? new(timestamps) : null;

    /// <summary>
    /// The application ID for the game.
    /// </summary>
    public ulong? ApplicationId => jsonModel.ApplicationId;

    /// <summary>
    /// What the player is currently doing.
    /// </summary>
    public string? Details => jsonModel.Details;

    /// <summary>
    /// The user's current party status or text description of activity.
    /// </summary>
    public string? State => jsonModel.State;

    /// <summary>
    /// The emoji used for a custom status.
    /// </summary>
    public Emoji? Emoji { get; } = jsonModel.Emoji is { } emoji ? Emoji.CreateFromJson(emoji, guildId, client) : null;

    /// <summary>
    /// Information for the current party of the player.
    /// </summary>
    public Party? Party { get; } = jsonModel.Party is { } party ? new(party) : null;

    /// <summary>
    /// Images for the presence and their hover texts.
    /// </summary>
    public UserActivityAssets? Assets { get; } = jsonModel.Assets is { } assets ? new(assets) : null;

    /// <summary>
    /// Secrets for Rich Presence joining and spectating.
    /// </summary>
    public UserActivitySecrets? Secrets { get; } = jsonModel.Secrets is { } secrets ? new(secrets) : null;

    /// <summary>
    /// Whether the activity is an instanced game session.
    /// </summary>
    public bool? Instance => jsonModel.Instance;

    /// <summary>
    /// Activity flags describing what the payload includes.
    /// </summary>
    public UserActivityFlags? Flags => jsonModel.Flags;

    /// <summary>
    /// The custom buttons shown in the Rich Presence (max 2).
    /// </summary>
    public IReadOnlyList<UserActivityButton> Buttons { get; } = jsonModel.ButtonsLabels.SelectOrEmpty(b => new UserActivityButton(b)).ToArray();

    /// <summary>
    /// The ID of the guild where this activity is associated.
    /// </summary>
    public ulong GuildId { get; } = guildId;
}
