using NetCord.Rest;

namespace NetCord;

public class GuildScheduledEvent : Entity, IJsonModel<JsonModels.JsonGuildScheduledEvent>
{
    JsonModels.JsonGuildScheduledEvent IJsonModel<JsonModels.JsonGuildScheduledEvent>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildScheduledEvent _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public Snowflake GuildId => _jsonModel.GuildId;

    public Snowflake? ChannelId => _jsonModel.ChannelId;

    public Snowflake? CreatorId => _jsonModel.CreatorId;

    public string Name => _jsonModel.Name;

    public string? Description => _jsonModel.Description;

    public DateTimeOffset ScheduledStartTime => _jsonModel.ScheduledStartTime;

    public DateTimeOffset? ScheduledEndTime => _jsonModel.ScheduledEndTime;

    public GuildScheduledEventPrivacyLevel PrivacyLevel => _jsonModel.PrivacyLevel;

    public GuildScheduledEventStatus Status => _jsonModel.Status;

    public GuildScheduledEventEntityType EntityType => _jsonModel.EntityType;

    public Snowflake? EntityId => _jsonModel.EntityId;

    public string? Location => _jsonModel.EntityMetadata?.Location;

    public User? Creator { get; }

    public int? UserCount => _jsonModel.UserCount;

    public GuildScheduledEvent(JsonModels.JsonGuildScheduledEvent jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (_jsonModel.Creator != null)
            Creator = new(_jsonModel.Creator, client);
    }
}