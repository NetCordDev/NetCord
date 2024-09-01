using NetCord.Rest;

namespace NetCord;

public partial class GuildScheduledEvent : ClientEntity, IJsonModel<JsonModels.JsonGuildScheduledEvent>
{
    JsonModels.JsonGuildScheduledEvent IJsonModel<JsonModels.JsonGuildScheduledEvent>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildScheduledEvent _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public ulong GuildId => _jsonModel.GuildId;

    public ulong? ChannelId => _jsonModel.ChannelId;

    public ulong? CreatorId => _jsonModel.CreatorId;

    public string Name => _jsonModel.Name;

    public string? Description => _jsonModel.Description;

    public DateTimeOffset ScheduledStartTime => _jsonModel.ScheduledStartTime;

    public DateTimeOffset? ScheduledEndTime => _jsonModel.ScheduledEndTime;

    public GuildScheduledEventPrivacyLevel PrivacyLevel => _jsonModel.PrivacyLevel;

    public GuildScheduledEventStatus Status => _jsonModel.Status;

    public GuildScheduledEventEntityType EntityType => _jsonModel.EntityType;

    public ulong? EntityId => _jsonModel.EntityId;

    public string? Location => _jsonModel.EntityMetadata?.Location;

    public User? Creator { get; }

    public int? UserCount => _jsonModel.UserCount;

    public string? CoverImageHash => _jsonModel.CoverImageHash;

    public GuildScheduledEventRecurrenceRule? RecurrenceRule { get; }

    public GuildScheduledEvent(JsonModels.JsonGuildScheduledEvent jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var creator = _jsonModel.Creator;
        if (creator is not null)
            Creator = new(creator, client);

        var recurrenceRule = _jsonModel.RecurrenceRule;
        if (recurrenceRule is not null)
            RecurrenceRule = new(recurrenceRule);
    }

    public bool HasCoverImage => CoverImageHash is not null;

    public ImageUrl? GetCoverImageUrl(ImageFormat format) => _jsonModel.CoverImageHash is string hash ? ImageUrl.GuildScheduledEventCover(Id, hash, format) : null;
}
