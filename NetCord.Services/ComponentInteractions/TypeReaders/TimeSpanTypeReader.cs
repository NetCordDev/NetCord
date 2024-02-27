using NetCord.Services.Helpers;

namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class TimeSpanTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return new(TimeSpanTypeReaderHelper.Read(input.ToString(), configuration.IgnoreCase, configuration.CultureInfo, parameter.Name));
    }
}
