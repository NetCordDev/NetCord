namespace NetCord.Gateway;

public class UserActivitySecrets : IJsonModel<JsonModels.JsonUserActivitySecrets>
{
    JsonModels.JsonUserActivitySecrets IJsonModel<JsonModels.JsonUserActivitySecrets>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonUserActivitySecrets _jsonModel;

    public string? Join => _jsonModel.Join;
    public string? Spectate => _jsonModel.Spectate;
    public string? Match => _jsonModel.Match;

    public UserActivitySecrets(JsonModels.JsonUserActivitySecrets jsonModel)
    {
        _jsonModel = jsonModel;
    }
}