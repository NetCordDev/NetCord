using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public interface ICommandsBuilder
{
    public CommandBuilder AddCommand(IEnumerable<string> aliases, Delegate handler);

    public CommandGroupBuilder AddCommandGroup(IEnumerable<string> aliases);

    public CommandGroupBuilder AddCommandGroup(IEnumerable<string> aliases, Action<CommandGroupBuilder> builder);

    public void Build();
}

public interface ICommandsBuilder<TContext> : ICommandsBuilder where TContext : ICommandContext;

internal class CommandsBuilder<TContext>(CommandService<TContext> service) : ICommandsBuilder<TContext> where TContext : ICommandContext
{
    private record Data(List<CommandBuilder> Commands, List<CommandGroupBuilder> CommandGroups)
    {
        public Data() : this([], [])
        {
        }
    }

    private Data _data = new();

    public CommandBuilder AddCommand(IEnumerable<string> aliases, Delegate handler)
    {
        CommandBuilder result = new(aliases, handler);
        _data.Commands.Add(result);
        return result;
    }

    public CommandGroupBuilder AddCommandGroup(IEnumerable<string> aliases)
    {
        CommandGroupBuilder result = new(aliases);
        _data.CommandGroups.Add(result);
        return result;
    }

    public CommandGroupBuilder AddCommandGroup(IEnumerable<string> aliases, Action<CommandGroupBuilder> builder)
    {
        CommandGroupBuilder result = new(aliases);
        builder(result);
        _data.CommandGroups.Add(result);
        return result;
    }

    public void Build()
    {
        var (builders, groupBuilders) = Interlocked.Exchange(ref _data, new());

        int buildersCount = builders.Count;

        for (int i = 0; i < buildersCount; i++)
            service.AddCommand(builders[i]);

        int groupBuildersCount = groupBuilders.Count;

        for (int i = 0; i < groupBuildersCount; i++)
            service.AddCommandGroup(groupBuilders[i]);
    }
}
