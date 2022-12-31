namespace NetCord.Services.Commands.TypeReaders;

public class TimeOnlyTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration) => Task.FromResult<object?>(TimeOnly.Parse(input.Span, configuration.CultureInfo));
}
