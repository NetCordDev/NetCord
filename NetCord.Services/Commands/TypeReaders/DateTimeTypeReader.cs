namespace NetCord.Services.Commands.TypeReaders;

public class DateTimeTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(string input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options) => Task.FromResult((object?)DateTime.Parse(input, options.CultureInfo));
}