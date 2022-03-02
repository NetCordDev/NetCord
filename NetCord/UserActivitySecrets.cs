namespace NetCord;

public class UserActivitySecrets
{
    private readonly JsonModels.JsonUserActivitySecrets _jsonEntity;

    public string? Join => _jsonEntity.Join;
    public string? Spectate => _jsonEntity.Spectate;
    public string? Match => _jsonEntity.Match;

    internal UserActivitySecrets(JsonModels.JsonUserActivitySecrets jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}