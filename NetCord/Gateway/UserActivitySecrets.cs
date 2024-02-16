namespace NetCord.Gateway;

public class UserActivitySecrets(JsonModels.JsonUserActivitySecrets jsonModel) : IJsonModel<JsonModels.JsonUserActivitySecrets>
{
    JsonModels.JsonUserActivitySecrets IJsonModel<JsonModels.JsonUserActivitySecrets>.JsonModel => jsonModel;

    public string? Join => jsonModel.Join;
    public string? Spectate => jsonModel.Spectate;
    public string? Match => jsonModel.Match;
}
