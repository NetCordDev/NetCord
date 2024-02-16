namespace NetCord;

public class ApplicationCommandInteractionDataOption(JsonModels.JsonApplicationCommandInteractionDataOption jsonModel) : IJsonModel<JsonModels.JsonApplicationCommandInteractionDataOption>
{
    JsonModels.JsonApplicationCommandInteractionDataOption IJsonModel<JsonModels.JsonApplicationCommandInteractionDataOption>.JsonModel => jsonModel;

    public string Name => jsonModel.Name;

    public ApplicationCommandOptionType Type => jsonModel.Type;

    public string? Value => jsonModel.Value;

    public IReadOnlyList<ApplicationCommandInteractionDataOption>? Options { get; } = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToArray();

    public bool Focused => jsonModel.Focused;
}
