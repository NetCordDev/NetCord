namespace NetCord.Services.Commands.TypeReaders;

public class TimeOnlyTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object> ReadAsync(string input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options) => Task.FromResult((object)TimeOnly.Parse(input, options.CultureInfo));
}