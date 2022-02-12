namespace NetCord;

public class Team : Entity
{
    private readonly JsonModels.JsonTeam _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public string? Icon => _jsonEntity.Icon;

    public IEnumerable<TeamUser> Users { get; }

    public string Name => _jsonEntity.Name;

    public DiscordId OwnerId => _jsonEntity.OwnerId;

    internal Team(JsonModels.JsonTeam jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        Users = jsonEntity.Users.SelectOrEmpty(m => new TeamUser(m, client));
    }
}

public enum MembershipState
{
    Invited = 1,
    Accepted = 2,
}