namespace NetCord.Gateway;

public class ApplicationCommandInteractionDataOption : IJsonModel<JsonModels.JsonApplicationCommandInteractionDataOption>
{
    JsonModels.JsonApplicationCommandInteractionDataOption IJsonModel<JsonModels.JsonApplicationCommandInteractionDataOption>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandInteractionDataOption _jsonModel;

    public string Name => _jsonModel.Name;

    public ApplicationCommandOptionType Type => _jsonModel.Type;

    public string? Value => _jsonModel.Value;

    public IEnumerable<ApplicationCommandInteractionDataOption>? Options { get; }

    public bool Focused => _jsonModel.Focused;

    public ApplicationCommandInteractionDataOption(JsonModels.JsonApplicationCommandInteractionDataOption jsonModel)
    {
        _jsonModel = jsonModel;
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o));
    }
}
