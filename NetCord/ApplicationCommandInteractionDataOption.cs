namespace NetCord;

/// <summary>
/// Contains information on an <see cref="ApplicationCommandInteraction"/> parameter.
/// </summary>
public class ApplicationCommandInteractionDataOption(JsonModels.JsonApplicationCommandInteractionDataOption jsonModel) : IJsonModel<JsonModels.JsonApplicationCommandInteractionDataOption>
{
    JsonModels.JsonApplicationCommandInteractionDataOption IJsonModel<JsonModels.JsonApplicationCommandInteractionDataOption>.JsonModel => jsonModel;

    /// <summary>
    /// The parameter's name.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The parameter's type.
    /// </summary>
    public ApplicationCommandOptionType Type => jsonModel.Type;

    /// <summary>
    /// The parameter's value, <see langword="null"/> if omitted. Never <see langword="null"/> for autocomplete interactions. 
    /// </summary>
    public string? Value => jsonModel.Value;

    /// <summary>
    /// A list of <see cref="ApplicationCommandInteractionDataOption"/> objects, if the option's <see cref="Type"/> is <see cref="ApplicationCommandOptionType.SubCommand"/> or <see cref="ApplicationCommandOptionType.SubCommandGroup"/>, otherwise empty.
    /// </summary>
    public IReadOnlyList<ApplicationCommandInteractionDataOption>? Options { get; } = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToArray();

    /// <summary>
    /// If the user is currently typing in this option. Used for autocomplete interactions.
    /// </summary>
    public bool Focused => jsonModel.Focused;
}
