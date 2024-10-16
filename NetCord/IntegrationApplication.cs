using NetCord.Rest;

namespace NetCord;

public class IntegrationApplication : Entity, IJsonModel<JsonModels.JsonIntegrationApplication>
{
    public IntegrationApplication(JsonModels.JsonIntegrationApplication jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;

        var bot = _jsonModel.Bot;
        if (bot is not null)
            Bot = new(bot, client);
    }

    JsonModels.JsonIntegrationApplication IJsonModel<JsonModels.JsonIntegrationApplication>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonIntegrationApplication _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public string? IconHash => _jsonModel.IconHash;

    public string Description => _jsonModel.Description;

    public string Summary => _jsonModel.Summary;

    public User? Bot { get; }
}
