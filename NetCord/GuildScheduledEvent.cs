using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a guild scheduled event.
/// </summary>
public partial class GuildScheduledEvent(JsonGuildScheduledEvent jsonModel, RestClient client) 
    : ClientEntity(client), IJsonModel<JsonGuildScheduledEvent>
{
    JsonGuildScheduledEvent IJsonModel<JsonGuildScheduledEvent>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the scheduled event.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The ID of the guild which the scheduled event belongs to.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The ID of the channel in which the scheduled event will be hosted, or <see langword="null"/> if it is an external event.
    /// </summary>
    public ulong? ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// The ID of the user that created the scheduled event.
    /// </summary>
    public ulong? CreatorId => jsonModel.CreatorId;

    /// <summary>
    /// The name of the scheduled event.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The description of the scheduled event.
    /// </summary>
    public string? Description => jsonModel.Description;

    /// <summary>
    /// The time the scheduled event will start.
    /// </summary>
    public DateTimeOffset ScheduledStartTime => jsonModel.ScheduledStartTime;

    /// <summary>
    /// The time the scheduled event will end, or <see langword="null"/> if not specified.
    /// </summary>
    /// <remarks>
    /// Required for external events (<see cref="GuildScheduledEventEntityType.External"/>).
    /// </remarks>
    public DateTimeOffset? ScheduledEndTime => jsonModel.ScheduledEndTime;

    /// <summary>
    /// The privacy level of the scheduled event.
    /// </summary>
    public GuildScheduledEventPrivacyLevel PrivacyLevel => jsonModel.PrivacyLevel;

    /// <summary>
    /// The status of the scheduled event.
    /// </summary>
    public GuildScheduledEventStatus Status => jsonModel.Status;

    /// <summary>
    /// The type of the scheduled event (e.g., Voice, Stage, or External).
    /// </summary>
    public GuildScheduledEventEntityType EntityType => jsonModel.EntityType;

    /// <summary>
    /// The ID of the internal entity associated with the event (e.g., a Stage instance ID).
    /// </summary>
    public ulong? EntityId => jsonModel.EntityId;

    /// <summary>
    /// The physical or digital location of the event, if it is an external event.
    /// </summary>
    public string? Location => jsonModel.EntityMetadata?.Location;

    /// <summary>
    /// The user that created the scheduled event, if available.
    /// </summary>
    public User? Creator { get; } = jsonModel.Creator is { } creator ? new(creator, client) : null;

    /// <summary>
    /// The number of users subscribed to the scheduled event, if provided.
    /// </summary>
    public int? UserCount => jsonModel.UserCount;

    /// <summary>
    /// The cover image hash of the scheduled event, or <see langword="null"/> if not set.
    /// </summary>
    public string? CoverImageHash => jsonModel.CoverImageHash;

    /// <summary>
    /// The recurrence rule for the scheduled event, defining how it repeats.
    /// </summary>
    public GuildScheduledEventRecurrenceRule? RecurrenceRule { get; } = jsonModel.RecurrenceRule is { } recurrenceRule ? new(recurrenceRule) : null;

    /// <summary>
    /// Gets a value indicating whether the scheduled event has a cover image set.
    /// </summary>
    public bool HasCoverImage => CoverImageHash is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the scheduled event's cover image.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the cover image, or <see langword="null"/> if the event does not have one.</returns>
    public ImageUrl? GetCoverImageUrl(ImageFormat format) => jsonModel.CoverImageHash is string hash ? ImageUrl.GuildScheduledEventCover(Id, hash, format) : null;
}
