using NetCord.Services.Helpers;

namespace NetCord.Services.Interactions.TypeReaders;

public class TimeSpanTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return new(TimeSpanTypeReaderHelper.Read(input.ToString(), configuration.IgnoreCase, configuration.CultureInfo, parameter.Name));
    }
}
