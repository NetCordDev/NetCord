namespace NetCord;

public class GuildInvite : IJsonModel<JsonModels.JsonGuildInvite>
{
    JsonModels.JsonGuildInvite IJsonModel<JsonModels.JsonGuildInvite>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildInvite _jsonModel;

    public GuildInvite(JsonModels.JsonGuildInvite jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Inviter != null)
            Inviter = new(jsonModel.Inviter, client);
        if (jsonModel.TargetUser != null)
            TargetUser = new(jsonModel.TargetUser, client);
        if (jsonModel.TargetApplication != null)
            TargetApplication = new(jsonModel.TargetApplication, client);
    }

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public string Code => _jsonModel.Code;

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public User? Inviter { get; }

    public int MaxAge => _jsonModel.MaxAge;

    public int MaxUses => _jsonModel.MaxUses;

    public GuildInviteTargetType? TargetType => _jsonModel.TargetType;

    public User? TargetUser { get; }

    public Application? TargetApplication { get; }

    public bool Temporary => _jsonModel.Temporary;

    public int Uses => _jsonModel.Uses;
}
