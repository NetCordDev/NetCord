using System.Globalization;
using System.Text.Json;

namespace NetCord.Rest;

public class ApplicationCommandOptionChoice : IJsonModel<JsonModels.JsonApplicationCommandOptionChoice>
{
    JsonModels.JsonApplicationCommandOptionChoice IJsonModel<JsonModels.JsonApplicationCommandOptionChoice>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandOptionChoice _jsonModel;

    /// <summary>
    /// Name of the choice (1-100 characters).
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// Translations of <see cref="Name"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonModel.NameLocalizations;

    /// <summary>
    /// String value for the choice.
    /// </summary>
    public string? ValueString { get; }

    /// <summary>
    /// Numeric value for the choice.
    /// </summary>
    public double? ValueNumeric { get; }

    /// <summary>
    /// Type of value of the choice.
    /// </summary>
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
