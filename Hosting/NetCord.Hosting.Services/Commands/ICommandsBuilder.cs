using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public interface ICommandsBuilder
{
    public CommandBuilder AddCommand(IEnumerable<string> aliases, Delegate handler);

    public void Build();
}

public interface ICommandsBuilder<TContext> : ICommandsBuilder where TContext : ICommandContext;

internal class CommandsBuilder<TContext>(CommandService<TContext> service) : ICommandsBuilder<TContext> where TContext : ICommandContext
{
    private List<CommandBuilder> _builders = [];

    public CommandBuilder AddCommand(IEnumerable<string> aliases, Delegate handler)
    {
        CommandBuilder result = new(aliases, handler);
        _builders.Add(result);
        return result;
    }

    public void Build()
    {
        var builders = _builders;
        int count = builders.Count;

        for (int i = 0; i < count; i++)
            service.AddCommand(builders[i]);

        _builders = [];
    }
}
