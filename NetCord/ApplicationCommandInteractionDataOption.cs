using System.Text.Json;

namespace NetCord;

public class ApplicationCommandInteractionDataOption : IJsonModel<JsonModels.JsonApplicationCommandInteractionDataOption>
{
    JsonModels.JsonApplicationCommandInteractionDataOption IJsonModel<JsonModels.JsonApplicationCommandInteractionDataOption>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandInteractionDataOption _jsonModel;

    public string Name => _jsonModel.Name;

    public ApplicationCommandOptionType Type => _jsonModel.Type;

    public string? Value { get; }

    public IEnumerable<ApplicationCommandInteractionDataOption>? Options { get; }

    public bool Focused => _jsonModel.Focused;

    public ApplicationCommandInteractionDataOption(JsonModels.JsonApplicationCommandInteractionDataOption jsonModel)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Value.HasValue)
        {
            var value = jsonModel.Value.GetValueOrDefault();
            Value = value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText();
        }
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o));
    }
}