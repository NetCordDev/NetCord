namespace NetCord.Services.ApplicationCommands;

[GenerateMethodsForProperties]
public abstract partial class ApplicationCommandBuilder
{
    public string Name { get; }

    public Permissions? DefaultGuildUserPermissions { get; set; }

    public IEnumerable<ApplicationIntegrationType>? IntegrationTypes { get; set; }

    public IEnumerable<InteractionContextType>? Contexts { get; set; }

    public bool Nsfw { get; set; }

    public bool Register { get; set; } = true;

    private protected ApplicationCommandBuilder(string name)
    {
        Name = name;
    }
}

[GenerateMethodsForProperties]
public partial class SlashCommandBuilder(string name, string description, Delegate handler) : ApplicationCommandBuilder(name)
{
    public string Description => description;

    public Delegate Handler => handler;
}

[GenerateMethodsForProperties]
public partial class SlashCommandGroupBuilder(string name, string description) : ApplicationCommandBuilder(name)
{
    internal readonly List<SubSlashCommandBuilder> _subCommandBuilders = [];
    internal readonly List<SubSlashCommandGroupBuilder> _subCommandGroupBuilders = [];

    public string Description => description;

    public SubSlashCommandBuilder AddSubCommand(string name, string description, Delegate handler)
    {
        SubSlashCommandBuilder result = new(name, description, handler);
        _subCommandBuilders.Add(result);
        return result;
    }

    public SubSlashCommandGroupBuilder AddSubCommandGroup(string name, string description)
    {
        SubSlashCommandGroupBuilder result = new(name, description);
        _subCommandGroupBuilders.Add(result);
        return result;
    }

    public SubSlashCommandGroupBuilder AddSubCommandGroup(string name, string description, Action<SubSlashCommandGroupBuilder> builder)
    {
        SubSlashCommandGroupBuilder result = new(name, description);
        builder(result);
        _subCommandGroupBuilders.Add(result);
        return result;
    }
}

[GenerateMethodsForProperties]
public partial class SubSlashCommandBuilder(string name, string description, Delegate handler)
{
    public string Name => name;

    public string Description => description;

    public Delegate Handler => handler;
}

[GenerateMethodsForProperties]
public partial class SubSlashCommandGroupBuilder(string name, string description)
{
    internal readonly List<SubSlashCommandBuilder> _subCommands = [];

    public string Name => name;
    public string Description => description;

    public SubSlashCommandBuilder AddSubCommand(string name, string description, Delegate handler)
    {
        SubSlashCommandBuilder result = new(name, description, handler);
        _subCommands.Add(result);
        return result;
    }
}

[GenerateMethodsForProperties]
public partial class UserCommandBuilder(string name, Delegate handler) : ApplicationCommandBuilder(name)
{
    public Delegate Handler => handler;
}

[GenerateMethodsForProperties]
public partial class MessageCommandBuilder(string name, Delegate handler) : ApplicationCommandBuilder(name)
{
    public Delegate Handler => handler;
}

[GenerateMethodsForProperties]
public partial class EntryPointCommandBuilder(string name, string description) : ApplicationCommandBuilder(name)
{
    public string Description => description;

    public Delegate? Handler { get; set; }
}
