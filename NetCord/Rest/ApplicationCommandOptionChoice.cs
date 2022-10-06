using System.Globalization;
using System.Text.Json;

namespace NetCord.Rest;

public class ApplicationCommandOptionChoice : IJsonModel<JsonModels.JsonApplicationCommandOptionChoice>
{
    JsonModels.JsonApplicationCommandOptionChoice IJsonModel<JsonModels.JsonApplicationCommandOptionChoice>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandOptionChoice _jsonModel;

    public string Name => _jsonModel.Name;

    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonModel.NameLocalizations;

    public string? ValueString { get; }

    public double? ValueNumeric { get; }

    public ApplicationCommandOptionChoiceValueType ValueType { get; }

    public ApplicationCommandOptionChoice(JsonModels.JsonApplicationCommandOptionChoice jsonModel)
    {
        _jsonModel = jsonModel;
        JsonElement value = jsonModel.Value;
        if (value.ValueKind == JsonValueKind.String)
        {
            ValueString = value.GetString()!;
            ValueType = ApplicationCommandOptionChoiceValueType.String;
        }
        else
        {
            ValueNumeric = value.GetDouble();
            ValueType = ApplicationCommandOptionChoiceValueType.Numeric;
        }
    }
}
