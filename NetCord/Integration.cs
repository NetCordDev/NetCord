using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a Discord guild integration (e.g., Twitch, YouTube, or Bot Applications).
/// </summary>
public class Integration(JsonIntegration jsonModel, RestClient client) : Entity, IJsonModel<JsonIntegration>
{
    JsonIntegration IJsonModel<JsonIntegration>.JsonModel => jsonModel;

    /// The ID of the integration.
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The name of the integration.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The type of integration.
    /// </summary>
    public IntegrationType Type => jsonModel.Type;

    /// <summary>
    /// Whether this integration is enabled.
    /// </summary>
    public bool Enabled => jsonModel.Enabled;

    /// <summary>
    /// Whether this integration is syncing.
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public bool? Syncing => jsonModel.Syncing;

    /// <summary>
    /// The ID of the role that this integration uses for "subscribers".
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public ulong? RoleId => jsonModel.RoleId;

    /// <summary>
    /// Whether emoticons should be synced for this integration (Twitch, YouTube, etc.).
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public bool? EnableEmoticons => jsonModel.EnableEmoticons;

    /// <summary>
    /// The behavior for expiring subscribers.
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public IntegrationExpireBehavior? ExpireBehavior => jsonModel.ExpireBehavior;

    /// <summary>
    /// The grace period (in days) before expiring subscribers.
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public int? ExpireGracePeriod => jsonModel.ExpireGracePeriod;

    /// <summary>
    /// The user for this integration.
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public User? User { get; } = jsonModel.User is { } user ? new(user, client) : null;

    /// <summary>
    /// The integration account information.
    /// </summary>
    public Account Account { get; } = new(jsonModel.Account);

    /// <summary>
    /// The date and time when this integration was last synced.
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public DateTimeOffset? SyncedAt => jsonModel.SyncedAt;

    /// <summary>
    /// How many subscribers this integration has.
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public int? SubscriberCount => jsonModel.SubscriberCount;

    /// <summary>
    /// Whether this integration has been revoked.
    /// </summary>
    /// <remarks>
    /// This property is not provided for bot integrations.
    /// </remarks>
    public bool? Revoked => jsonModel.Revoked;

    /// <summary>
    /// The bot application for Discord integrations.
    /// </summary>
    public IntegrationApplication? Application { get; } = jsonModel.Application is { } application ? new(application, client) : null;
}
