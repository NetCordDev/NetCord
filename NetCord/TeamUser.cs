using NetCord.Rest;

namespace NetCord;

public class TeamUser : User, IJsonModel<JsonModels.JsonTeamUser>
{
    JsonModels.JsonTeamUser IJsonModel<JsonModels.JsonTeamUser>.JsonModel => _jsonTeamModel;
    private readonly JsonModels.JsonTeamUser _jsonTeamModel;

    public MembershipState MembershipState => _jsonTeamModel.MembershipState;
    public ulong TeamId => _jsonTeamModel.TeamId;
    public TeamRole Role => _jsonTeamModel.Role;

    public TeamUser(JsonModels.JsonTeamUser jsonModel, RestClient client) : base(jsonModel.User, client)
    {
        _jsonTeamModel = jsonModel;
    }
}
