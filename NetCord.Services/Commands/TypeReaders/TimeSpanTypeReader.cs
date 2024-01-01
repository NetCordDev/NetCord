using NetCord.Services.Helpers;

namespace NetCord.Services.Commands.TypeReaders;

public partial class TimeSpanTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return new(TimeSpanTypeReaderHelper.Read(input.ToString(), configuration.IgnoreCase, configuration.CultureInfo, parameter.Name));
    }
}
