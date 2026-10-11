using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Contains information on an <see cref="ApplicationCommandInteraction"/> parameter.
/// </summary>
public class ApplicationCommandInteractionDataOption(JsonApplicationCommandInteractionDataOption jsonModel)
{
    /// <summary>
    /// The parameter's name.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// The parameter's type.
    /// </summary>
    public ApplicationCommandOptionType Type { get; } = jsonModel.Type;

    /// <summary>
    /// The parameter's value, <see langword="null"/> if omitted. When autocomplete is triggered for this parameter, the value is guaranteed to be non-<see langword="null"/>.
    /// </summary>
    public string? Value { get; } = jsonModel.Value;

    /// <summary>
    /// A list of <see cref="ApplicationCommandInteractionDataOption"/> objects, if the option's <see cref="Type"/> is <see cref="ApplicationCommandOptionType.SubCommand"/> or <see cref="ApplicationCommandOptionType.SubCommandGroup"/>, otherwise empty.
    /// </summary>
    public IReadOnlyList<ApplicationCommandInteractionDataOption>? Options { get; } = [.. jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o))];

    /// <summary>
    /// If the user is currently typing in this option. Used for autocomplete interactions.
    /// </summary>
    public bool Focused { get; } = jsonModel.Focused.GetValueOrDefault();
}
