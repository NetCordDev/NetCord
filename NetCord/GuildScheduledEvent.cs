using NetCord.Rest;

namespace NetCord;

public partial class GuildScheduledEvent(JsonModels.JsonGuildScheduledEvent jsonModel, RestClient client) : ClientEntity(jsonModel, client)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public ulong? ChannelId { get; } = jsonModel.ChannelId;

    public ulong? CreatorId { get; } = jsonModel.CreatorId;

    public string Name { get; } = jsonModel.Name;

    public string? Description { get; } = jsonModel.Description;

    public DateTimeOffset ScheduledStartTime { get; } = jsonModel.ScheduledStartTime;

    public DateTimeOffset? ScheduledEndTime { get; } = jsonModel.ScheduledEndTime;

    public GuildScheduledEventPrivacyLevel PrivacyLevel { get; } = jsonModel.PrivacyLevel;

    public GuildScheduledEventStatus Status { get; } = jsonModel.Status;

    public GuildScheduledEventEntityType EntityType { get; } = jsonModel.EntityType;

    public ulong? EntityId { get; } = jsonModel.EntityId;

    public GuildScheduledEventEntityMetadata? EntityMetadata { get; } = jsonModel.EntityMetadata is { } entityMetadata ? new(entityMetadata) : null;

    public User? Creator { get; } = jsonModel.Creator is { } creator ? new(creator, client) : null;

    public int? UserCount { get; } = jsonModel.UserCount;

    public string? CoverImageHash { get; } = jsonModel.CoverImageHash;

    public GuildScheduledEventRecurrenceRule? RecurrenceRule { get; } = jsonModel.RecurrenceRule is { } recurrenceRule ? new(recurrenceRule) : null;

    public bool HasCoverImage => CoverImageHash is not null;

    public ImageUrl? GetCoverImageUrl(ImageFormat format) => jsonModel.CoverImageHash is string hash ? ImageUrl.GuildScheduledEventCover(Id, hash, format) : null;
}
