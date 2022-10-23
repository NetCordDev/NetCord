using NetCord.Rest;

namespace NetCord;

public class Team : Entity, IJsonModel<JsonModels.JsonTeam>
{
    JsonModels.JsonTeam IJsonModel<JsonModels.JsonTeam>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonTeam _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string? Icon => _jsonModel.Icon;

    public IEnumerable<TeamUser> Users { get; }

    public string Name => _jsonModel.Name;

    public ulong OwnerId => _jsonModel.OwnerId;

    public Team(JsonModels.JsonTeam jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Users = jsonModel.Users.SelectOrEmpty(m => new TeamUser(m, client));
    }
}
