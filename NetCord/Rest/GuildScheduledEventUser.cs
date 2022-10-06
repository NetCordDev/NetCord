namespace NetCord.Rest;

public class GuildScheduledEventUser : IJsonModel<JsonModels.JsonGuildScheduledEventUser>
{
    JsonModels.JsonGuildScheduledEventUser IJsonModel<JsonModels.JsonGuildScheduledEventUser>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildScheduledEventUser _jsonModel;

    public Snowflake ScheduledEventId => _jsonModel.ScheduledEventId;

    public User User { get; }

    public GuildScheduledEventUser(JsonModels.JsonGuildScheduledEventUser jsonModel, Snowflake guildId, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.GuildUser != null)
        {
            jsonModel.GuildUser.User = jsonModel.User;
            User = new GuildUser(jsonModel.GuildUser, guildId, client);
        }
        else
            User = new(jsonModel.User, client);
    }
}
