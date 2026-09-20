namespace NetCord.Rest;

/// <summary>
/// Represents an application command (slash command, user command, or message command) within Discord.
/// </summary>
public partial class ApplicationCommand(JsonModels.JsonApplicationCommand jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonApplicationCommand>
{
    JsonModels.JsonApplicationCommand IJsonModel<JsonModels.JsonApplicationCommand>.JsonModel => jsonModel;
    
    /// <inheritdoc />
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// Type of the command.
    /// </summary>
    public ApplicationCommandType Type => jsonModel.Type;

    /// <summary>
    /// ID of the parent application.
    /// </summary>
    public ulong ApplicationId => jsonModel.ApplicationId;

    /// <summary>
    /// Name of the command (1-32 characters).
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// Localizations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations => jsonModel.NameLocalizations;

    /// <summary>
    /// Description of the command (1-100 characters).
    /// </summary>
    public string Description => jsonModel.Description;

    /// <summary>
    /// Localizations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations => jsonModel.DescriptionLocalizations;

    /// <summary>
    /// Default required permissions to use the command.
    /// </summary>
    public Permissions? DefaultGuildPermissions => jsonModel.DefaultGuildPermissions;

    /// <summary>
    /// Parameters for the command (max 25).
    /// </summary>
    public IReadOnlyList<ApplicationCommandOption> Options { get; } = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandOption(o, jsonModel.Name, jsonModel.Id)).ToArray();

    /// <summary>
    /// Indicates whether the command is age-restricted.
    /// </summary>
    public bool Nsfw => jsonModel.Nsfw;

    /// <summary>
    /// Installation context(s) where the command is available, only for globally-scoped commands.
    /// </summary>
    public IReadOnlyList<ApplicationIntegrationType>? IntegrationTypes => jsonModel.IntegrationTypes;

    /// <summary>
    /// Interaction context(s) where the command can be used, only for globally-scoped commands.
    /// </summary>
    public IReadOnlyList<InteractionContextType>? Contexts => jsonModel.Contexts;

    /// <summary>
    /// Autoincrementing version identifier updated during substantial record changes.
    /// </summary>
    public ulong Version => jsonModel.Version;

    public override string ToString() => string.Create(null, $"</{Name}:{Id}>");

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        return destination.TryWrite(provider, $"</{Name}:{Id}>", out charsWritten);
    }
}
