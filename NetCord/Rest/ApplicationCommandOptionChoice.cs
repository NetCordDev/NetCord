using System.Globalization;
using System.Text.Json;

namespace NetCord.Rest;

public class ApplicationCommandOptionChoice : IJsonModel<JsonModels.JsonApplicationCommandOptionChoice>
{
    JsonModels.JsonApplicationCommandOptionChoice IJsonModel<JsonModels.JsonApplicationCommandOptionChoice>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandOptionChoice _jsonModel;

    public string Name => _jsonModel.Name;

    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonModel.NameLocalizations;

    public string Value { get; }

    public ApplicationCommandOptionChoice(JsonModels.JsonApplicationCommandOptionChoice jsonModel)
    {
        _jsonModel = jsonModel;
        JsonElement value = jsonModel.Value;
        Value = value.ValueKind == JsonValueKind.String ? value.GetString()! : value.GetRawText();
    }
}