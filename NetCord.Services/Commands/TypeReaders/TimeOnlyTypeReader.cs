namespace NetCord.Services.Commands.TypeReaders;

public class TimeOnlyTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(TimeOnly.Parse(input.Span, configuration.CultureInfo));
}
