using System.Text.Json;

using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a choice for an application command option.
/// </summary>
public class ApplicationCommandOptionChoice(JsonApplicationCommandOptionChoice jsonModel) : IJsonModel<JsonApplicationCommandOptionChoice>
{
    JsonApplicationCommandOptionChoice IJsonModel<JsonApplicationCommandOptionChoice>.JsonModel => jsonModel;

    /// <summary>
    /// Name of the choice (1-100 characters).
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// Localizations of <see cref="Name"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations => jsonModel.NameLocalizations;

    /// <summary>
    /// String value for the choice.
    /// </summary>
    public string? ValueString { get; } = jsonModel.Value.ValueKind is JsonValueKind.String ? jsonModel.Value.GetString()! : null;

    /// <summary>
    /// Numeric value for the choice.
    /// </summary>
    public double? ValueNumeric { get; } = jsonModel.Value.ValueKind is not JsonValueKind.String ? jsonModel.Value.GetDouble() : null;

    /// <summary>
    /// Type of value of the choice.
    /// </summary>
    public ApplicationCommandOptionChoiceValueType ValueType { get; } = jsonModel.Value.ValueKind is JsonValueKind.String
        ? ApplicationCommandOptionChoiceValueType.String
        : ApplicationCommandOptionChoiceValueType.Numeric;
}
