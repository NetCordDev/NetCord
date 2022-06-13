namespace NetCord;

public class Presence : IJsonModel<JsonModels.JsonPresence>
{
    JsonModels.JsonPresence IJsonModel<JsonModels.JsonPresence>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonPresence _jsonModel;

    public User User { get; }
    public Snowflake GuildId => _jsonModel.GuildId;
    public UserStatusType Status => _jsonModel.Status;
    public IEnumerable<UserActivity> Activities { get; }
    public IReadOnlyDictionary<Platform, UserStatusType> Platform => _jsonModel.Platform;

    public Presence(JsonModels.JsonPresence jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(jsonModel.User, client);
        Activities = jsonModel.Activities.SelectOrEmpty(a => new UserActivity(a, GuildId, client));
    }
}