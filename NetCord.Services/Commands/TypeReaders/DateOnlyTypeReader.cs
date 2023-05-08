namespace NetCord.Services.Commands.TypeReaders;

public class DateOnlyTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => Task.FromResult<object?>(DateOnly.Parse(input.Span, configuration.CultureInfo));
}
