using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class ApplicationCommand(JsonApplicationCommand jsonModel, RestClient client) : ClientEntity(jsonModel, client), ISpanFormattable
{
    /// <summary>
    /// Type of the command.
    /// </summary>
    public ApplicationCommandType Type { get; } = jsonModel.Type.GetValueOrDefault(ApplicationCommandType.ChatInput);

    /// <summary>
    /// ID of the parent application.
    /// </summary>
    public ulong ApplicationId { get; } = jsonModel.ApplicationId;

    /// <summary>
    /// Name of the command (1-32 characters).
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// Localizations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations { get; } = jsonModel.NameLocalizations;

    /// <summary>
    /// Description of the command (1-100 characters).
    /// </summary>
    public string Description { get; } = jsonModel.Description;

    /// <summary>
    /// Localizations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations { get; } = jsonModel.DescriptionLocalizations;

    /// <summary>
    /// Default required permissions to use the command.
    /// </summary>
    public Permissions? DefaultGuildPermissions { get; } = jsonModel.DefaultGuildPermissions;

    /// <summary>
    /// Parameters for the command (max 25).
    /// </summary>
    public IReadOnlyList<ApplicationCommandOption>? Options { get; } = CreateOptions(jsonModel);

    private static IReadOnlyList<ApplicationCommandOption>? CreateOptions(JsonApplicationCommand jsonModel)
    {
        if (jsonModel.Options is { } options)
        {
            var name = jsonModel.Name;
            var id = jsonModel.Id;

            return [.. options.Select(o => new ApplicationCommandOption(o, name, id))];
        }

        return null;
    }

    /// <summary>
    /// Indicates whether the command is age-restricted.
    /// </summary>
    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

    /// <summary>
    /// Installation context(s) where the command is available, only for globally-scoped commands.
    /// </summary>
    public IReadOnlyList<ApplicationIntegrationType>? IntegrationTypes { get; } = jsonModel.IntegrationTypes;

    /// <summary>
    /// Interaction context(s) where the command can be used, only for globally-scoped commands.
    /// </summary>
    public IReadOnlyList<InteractionContextType>? Contexts { get; } = jsonModel.Contexts;

    /// <summary>
    /// Autoincrementing version identifier updated during substantial record changes.
    /// </summary>
    public ulong Version { get; } = jsonModel.Version;

    /// <summary>
    /// Determines whether the interaction is handled by the application's interactions handler or by Discord.
    /// </summary>
    public EntryPointCommandHandlerType? Handler { get; } = jsonModel.Handler;

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
