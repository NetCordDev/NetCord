namespace NetCord.Services.ApplicationCommands;

/// <inheritdoc cref="ApplicationCommandAttribute" />
[GenerateMethodsForProperties]
public abstract partial class ApplicationCommandBuilder
{
    /// <inheritdoc cref="ApplicationCommandAttribute.Name" />
    public string Name { get; }

    /// <inheritdoc cref="ApplicationCommandAttribute.DefaultGuildPermissions" />
    public Permissions? DefaultGuildPermissions { get; set; }

    /// <inheritdoc cref="ApplicationCommandAttribute.IntegrationTypes" />
    public IEnumerable<ApplicationIntegrationType>? IntegrationTypes { get; set; }

    /// <inheritdoc cref="ApplicationCommandAttribute.Contexts" />
    public IEnumerable<InteractionContextType>? Contexts { get; set; }

    /// <inheritdoc cref="ApplicationCommandAttribute.Nsfw" />
    public bool Nsfw { get; set; }

    /// <inheritdoc cref="ApplicationCommandAttribute.Register" />
    public bool Register { get; set; } = true;

    private protected ApplicationCommandBuilder(string name)
    {
        Name = name;
    }
}

/// <inheritdoc cref="SlashCommandAttribute" />
/// <param name="name" />
/// <param name="description" />
/// <param name="handler"><inheritdoc cref="Handler" path="/summary" /></param>
[GenerateMethodsForProperties]
public partial class SlashCommandBuilder(string name, string description, Delegate handler) : ApplicationCommandBuilder(name)
{
    /// <inheritdoc cref="SlashCommandAttribute.Description" />
    public string Description => description;

    /// <summary>
    /// Handler that represents the body of the command.
    /// </summary>
    public Delegate Handler => handler;
}

/// <inheritdoc cref="SlashCommandAttribute" />
/// <param name="name" />
/// <param name="description" />
[GenerateMethodsForProperties]
public partial class SlashCommandGroupBuilder(string name, string description) : ApplicationCommandBuilder(name)
{
    internal readonly List<SubSlashCommandBuilder> _subCommandBuilders = [];
    internal readonly List<SubSlashCommandGroupBuilder> _subCommandGroupBuilders = [];

    /// <inheritdoc cref="SlashCommandAttribute.Description" />
    public string Description => description;

    /// <summary>
    /// Adds a sub command to a command group.
    /// </summary>
    /// <param name="name"><inheritdoc cref="SubSlashCommandBuilder.Name" path="/summary" /></param>
    /// <param name="description"><inheritdoc cref="SubSlashCommandBuilder.Description" path="/summary" /></param>
    /// <param name="handler"><inheritdoc cref="SubSlashCommandBuilder.Handler" path="/summary" /></param>
    /// <returns></returns>
    public SubSlashCommandBuilder AddSubCommand(string name, string description, Delegate handler)
    {
        SubSlashCommandBuilder result = new(name, description, handler);
        _subCommandBuilders.Add(result);
        return result;
    }

    /// <summary>
    /// Adds a sub command group to a command group.
    /// </summary>
    /// <param name="name"><inheritdoc cref="SubSlashCommandGroupBuilder.Name" path="/summary" /></param>
    /// <param name="description"><inheritdoc cref="SubSlashCommandGroupBuilder.Description" path="/summary" /></param>
    /// <returns></returns>
    public SubSlashCommandGroupBuilder AddSubCommandGroup(string name, string description)
    {
        SubSlashCommandGroupBuilder result = new(name, description);
        _subCommandGroupBuilders.Add(result);
        return result;
    }

    /// <inheritdoc cref="AddSubCommandGroup(string, string)" />
    /// <param name="name" />
    /// <param name="description" />
    /// <param name="builder">A delegate that builds the sub command group.</param>
    public SubSlashCommandGroupBuilder AddSubCommandGroup(string name, string description, Action<SubSlashCommandGroupBuilder> builder)
    {
        SubSlashCommandGroupBuilder result = new(name, description);
        builder(result);
        _subCommandGroupBuilders.Add(result);
        return result;
    }
}

/// <inheritdoc cref="SlashCommandAttribute" />
/// <param name="name" />
/// <param name="description" />
/// <param name="handler"><inheritdoc cref="Handler" path="/summary" /></param>
[GenerateMethodsForProperties]
public partial class SubSlashCommandBuilder(string name, string description, Delegate handler)
{
    /// <inheritdoc cref="ApplicationCommandBuilder.Name" />
    public string Name => name;

    /// <inheritdoc cref="SlashCommandBuilder.Description" />
    public string Description => description;

    /// <summary>
    /// Handler that represents the body of the command.
    /// </summary>
    public Delegate Handler => handler;
}

/// <inheritdoc cref="SlashCommandAttribute" />
/// <param name="name" />
/// <param name="description" />
[GenerateMethodsForProperties]
public partial class SubSlashCommandGroupBuilder(string name, string description)
{
    internal readonly List<SubSlashCommandBuilder> _subCommands = [];

    /// <inheritdoc cref="ApplicationCommandBuilder.Name" />
    public string Name => name;

    /// <inheritdoc cref="SlashCommandBuilder.Description" />
    public string Description => description;

    /// <inheritdoc cref="SlashCommandGroupBuilder.AddSubCommand(string, string, Delegate)" />
    public SubSlashCommandBuilder AddSubCommand(string name, string description, Delegate handler)
    {
        SubSlashCommandBuilder result = new(name, description, handler);
        _subCommands.Add(result);
        return result;
    }
}

/// <inheritdoc cref="UserCommandAttribute" />
/// <param name="name" />
/// <param name="handler"><inheritdoc cref="Handler" path="/summary" /></param>
[GenerateMethodsForProperties]
public partial class UserCommandBuilder(string name, Delegate handler) : ApplicationCommandBuilder(name)
{
    /// <summary>
    /// Handler that represents the body of the command.
    /// </summary>
    public Delegate Handler => handler;
}

/// <inheritdoc cref="MessageCommandAttribute" />
/// <param name="name" />
/// <param name="handler"><inheritdoc cref="Handler" path="/summary" /></param>
[GenerateMethodsForProperties]
public partial class MessageCommandBuilder(string name, Delegate handler) : ApplicationCommandBuilder(name)
{
    /// <summary>
    /// Handler that represents the body of the command.
    /// </summary>
    public Delegate Handler => handler;
}

/// <inheritdoc cref="EntryPointCommandAttribute" />
/// <param name="name" />
/// <param name="description" />
[GenerateMethodsForProperties]
public partial class EntryPointCommandBuilder(string name, string description) : ApplicationCommandBuilder(name)
{
    /// <inheritdoc cref="EntryPointCommandAttribute.Description" />
    public string Description => description;

    /// <summary>
    /// Handler that represents the body of the command.
    /// </summary>
    public Delegate? Handler { get; set; }
}
