namespace NetCord.Rest;

public class GuildScheduledEventUser : IJsonModel<JsonModels.JsonGuildScheduledEventUser>
{
    JsonModels.JsonGuildScheduledEventUser IJsonModel<JsonModels.JsonGuildScheduledEventUser>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildScheduledEventUser _jsonModel;

    public ulong ScheduledEventId => _jsonModel.ScheduledEventId;

    public User User { get; }

    public GuildScheduledEventUser(JsonModels.JsonGuildScheduledEventUser jsonModel, ulong guildId, RestClient client)
    {
        _jsonModel = jsonModel;

        var guildUser = jsonModel.GuildUser;
        if (guildUser is null)
            User = new(jsonModel.User, client);
        else
        {
            guildUser.User = jsonModel.User;
            User = new GuildUser(guildUser, guildId, client);
        }
    }
}
