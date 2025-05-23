namespace NetCord.Services.ApplicationCommands;

public interface ISlashCommandBuilder
{
    public void AddSubCommand(string name, string description, Delegate handler);
}

public class SubSlashCommandBuilder : ISlashCommandBuilder
{
    internal List<SubCommand> SubCommands { get; } = [];

    public void AddSubCommand(string name, string description, Delegate handler)
    {
        SubCommands.Add(new(name, description, handler));
    }
}

public class SlashCommandBuilder : ISlashCommandBuilder
{
    internal List<SubCommand> SubCommands { get; } = [];

    internal List<SubCommandGroup> SubCommandGroups { get; } = [];

    public void AddSubCommand(string name, string description, Delegate handler)
    {
        SubCommands.Add(new(name, description, handler));
    }

    public void AddSubCommand(string name, string description, Action<SubSlashCommandBuilder> builder)
    {
        SubCommandGroups.Add(new(name, description, builder));
    }
}

internal record SubCommand(string Name, string Description, Delegate Handler);

internal record SubCommandGroup(string Name, string Description, Action<SubSlashCommandBuilder> Builder);
