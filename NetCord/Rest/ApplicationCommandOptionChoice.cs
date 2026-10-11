using System.Text.Json;

using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class ApplicationCommandOptionChoice
{
    /// <summary>
    /// Name of the choice (1-100 characters).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Localizations of <see cref="Name"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations { get; }

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

    public ApplicationCommandOptionChoice(JsonApplicationCommandOptionChoice jsonModel)
    {
        Name = jsonModel.Name;
        NameLocalizations = jsonModel.NameLocalizations;

        var value = jsonModel.Value;
        if (value.ValueKind is JsonValueKind.String)
        {
            ValueString = value.GetString();
            ValueType = ApplicationCommandOptionChoiceValueType.String;
        }
        else
        {
            ValueNumeric = value.GetDouble();
            ValueType = ApplicationCommandOptionChoiceValueType.Numeric;
        }
    }
}
