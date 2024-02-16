using NetCord.Rest;

namespace NetCord;

public class TeamUser(JsonModels.JsonTeamUser jsonModel, RestClient client) : User(jsonModel.User, client), IJsonModel<JsonModels.JsonTeamUser>
{
    JsonModels.JsonTeamUser IJsonModel<JsonModels.JsonTeamUser>.JsonModel => jsonModel;

    public MembershipState MembershipState => jsonModel.MembershipState;
    public ulong TeamId => jsonModel.TeamId;
    public TeamRole Role => jsonModel.Role;
}
