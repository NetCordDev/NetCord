using NetCord.Rest;

namespace NetCord;

public class Team(JsonModels.JsonTeam jsonModel, RestClient client) : Entity, IJsonModel<JsonModels.JsonTeam>
{
    JsonModels.JsonTeam IJsonModel<JsonModels.JsonTeam>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    public string? IconHash => jsonModel.IconHash;

    public IReadOnlyList<TeamUser> Users { get; } = jsonModel.Users.SelectOrEmpty(m => new TeamUser(m, client)).ToArray();

    public string Name => jsonModel.Name;

    public ulong OwnerId => jsonModel.OwnerId;
}
