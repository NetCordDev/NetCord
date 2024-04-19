namespace NetCord.Rest;

public partial class ApplicationCommand(JsonModels.JsonApplicationCommand jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonApplicationCommand>
{
    JsonModels.JsonApplicationCommand IJsonModel<JsonModels.JsonApplicationCommand>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonApplicationCommand _jsonModel = jsonModel;

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Type of the command.
    /// </summary>
    public ApplicationCommandType Type => _jsonModel.Type;

    /// <summary>
    /// Id of the parent application.
    /// </summary>
    public ulong ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// Name of the command (1-32 characters).
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// Localizations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations => _jsonModel.NameLocalizations;

    /// <summary>
    /// Description of the command (1-100 characters).
    /// </summary>
    public string Description => _jsonModel.Description;

    /// <summary>
    /// Localizations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations => _jsonModel.DescriptionLocalizations;

    /// <summary>
    /// Default required permissions to use the command.
    /// </summary>
    public Permissions? DefaultGuildUserPermissions => _jsonModel.DefaultGuildUserPermissions;

    /// <summary>
    /// Indicates whether the command is available in DMs with the app.
    /// </summary>
    [Obsolete($"Replaced by '{nameof(Contexts)}'.")]
    public bool DMPermission => _jsonModel.DMPermission.GetValueOrDefault();

    /// <summary>
    /// Parameters for the command (max 25).
    /// </summary>
    public IReadOnlyList<ApplicationCommandOption> Options { get; } = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandOption(o, jsonModel.Name, jsonModel.Id)).ToArray();

    /// <summary>
    /// Indicates whether the command is enabled by default when the app is added to a guild.
    /// </summary>
    [Obsolete($"Replaced by '{nameof(DefaultGuildUserPermissions)}'.")]
    public bool DefaultPermission => _jsonModel.DefaultPermission;

    /// <summary>
    /// Indicates whether the command is age-restricted.
    /// </summary>
    public bool Nsfw => _jsonModel.Nsfw;

    /// <summary>
    /// Installation context(s) where the command is available, only for globally-scoped commands.
    /// </summary>
    public IReadOnlyList<ApplicationIntegrationType>? IntegrationTypes => _jsonModel.IntegrationTypes;

    /// <summary>
    /// Interaction context(s) where the command can be used, only for globally-scoped commands.
    /// </summary>
    public IReadOnlyList<InteractionContextType>? Contexts => _jsonModel.Contexts;

    /// <summary>
    /// Autoincrementing version identifier updated during substantial record changes.
    /// </summary>
    public ulong Version => _jsonModel.Version;

    public override string ToString() => $"</{Name}:{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var requiredLength = 5 + Name.Length;
        if (destination.Length < requiredLength || !Id.TryFormat(destination[(3 + Name.Length)..^1], out int length))
        {
            charsWritten = 0;
            return false;
        }

        "</".CopyTo(destination);
        Name.CopyTo(destination[2..]);
        destination[2 + Name.Length] = ':';
        destination[3 + Name.Length + length] = '>';

        charsWritten = 4 + Name.Length + length;
        return true;
    }
}
