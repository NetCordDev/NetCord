using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public interface IApplicationCommandsBuilder
{
    public SlashCommandBuilder AddSlashCommand(string name, string description, Delegate handler);

    public SlashCommandGroupBuilder AddSlashCommandGroup(string name, string description);

    public SlashCommandGroupBuilder AddSlashCommandGroup(string name, string description, Action<SlashCommandGroupBuilder> builder);

    public UserCommandBuilder AddUserCommand(string name, Delegate handler);

    public MessageCommandBuilder AddMessageCommand(string name, Delegate handler);

    public EntryPointCommandBuilder AddEntryPointCommand(string name, string description);

    public void Build();
}

public interface IApplicationCommandsBuilder<TContext> : IApplicationCommandsBuilder where TContext : IApplicationCommandContext;

internal class ApplicationCommandsBuilder<TContext>(IApplicationCommandService service) : IApplicationCommandsBuilder<TContext> where TContext : IApplicationCommandContext
{
    private List<ApplicationCommandBuilder> _builders = [];

    public SlashCommandBuilder AddSlashCommand(string name, string description, Delegate handler)
    {
        SlashCommandBuilder result = new(name, description, handler);
        _builders.Add(result);
        return result;
    }

    public SlashCommandGroupBuilder AddSlashCommandGroup(string name, string description)
    {
        SlashCommandGroupBuilder result = new(name, description);
        _builders.Add(result);
        return result;
    }

    public SlashCommandGroupBuilder AddSlashCommandGroup(string name, string description, Action<SlashCommandGroupBuilder> builder)
    {
        SlashCommandGroupBuilder result = new(name, description);
        builder(result);
        _builders.Add(result);
        return result;
    }

    public UserCommandBuilder AddUserCommand(string name, Delegate handler)
    {
        UserCommandBuilder result = new(name, handler);
        _builders.Add(result);
        return result;
    }

    public MessageCommandBuilder AddMessageCommand(string name, Delegate handler)
    {
        MessageCommandBuilder result = new(name, handler);
        _builders.Add(result);
        return result;
    }

    public EntryPointCommandBuilder AddEntryPointCommand(string name, string description)
    {
        EntryPointCommandBuilder result = new(name, description);
        _builders.Add(result);
        return result;
    }

    public void Build()
    {
        var builders = _builders;
        int count = builders.Count;

        for (int i = 0; i < count; i++)
        {
            var builder = builders[i];
            switch (builder)
            {
                case SlashCommandBuilder slashCommandBuilder:
                    service.AddSlashCommand(slashCommandBuilder);
                    break;
                case SlashCommandGroupBuilder slashCommandGroupBuilder:
                    service.AddSlashCommandGroup(slashCommandGroupBuilder);
                    break;
                case UserCommandBuilder userCommandBuilder:
                    service.AddUserCommand(userCommandBuilder);
                    break;
                case MessageCommandBuilder messageCommandBuilder:
                    service.AddMessageCommand(messageCommandBuilder);
                    break;
                case EntryPointCommandBuilder entryPointCommandBuilder:
                    service.AddEntryPointCommand(entryPointCommandBuilder);
                    break;
            }
        }

        _builders = [];
    }
}
