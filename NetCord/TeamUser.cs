using NetCord.Rest;

namespace NetCord;

public class TeamUser : User, IJsonModel<JsonModels.JsonTeamUser>
{
    JsonModels.JsonTeamUser IJsonModel<JsonModels.JsonTeamUser>.JsonModel => _jsonTeamModel;
    private readonly JsonModels.JsonTeamUser _jsonTeamModel;

    public MembershipState MembershipState => _jsonTeamModel.MembershipState;
    public IReadOnlyList<string> Permissions => _jsonTeamModel.Permissions;
    public ulong TeamId => _jsonTeamModel.TeamId;

    public TeamUser(JsonModels.JsonTeamUser jsonModel, RestClient client) : base(jsonModel.User, client)
    {
        _jsonTeamModel = jsonModel;
    }
}
