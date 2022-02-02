namespace NetCord.Services.Interactions.TypeReaders;

public class DateTimeTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object> ReadAsync(string input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options) => Task.FromResult((object)DateTime.Parse(input, options.CultureInfo));
}