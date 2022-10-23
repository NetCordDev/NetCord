namespace NetCord.Rest;

public class IntegrationApplication : Entity, IJsonModel<JsonModels.JsonIntegrationApplication>
{
    JsonModels.JsonIntegrationApplication IJsonModel<JsonModels.JsonIntegrationApplication>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonIntegrationApplication _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public string? IconHash => _jsonModel.IconHash;

    public string Description => _jsonModel.Description;

    public string Summary => _jsonModel.Summary;

    public User? Bot { get; }

    public IntegrationApplication(JsonModels.JsonIntegrationApplication jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (_jsonModel.Bot != null)
            Bot = new(_jsonModel.Bot, client);
    }
}
