namespace NetCord.Services.Commands.TypeReaders;

public class CategoryGuildChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is not null)
            return new(GetGuildChannel<CategoryGuildChannel>(guild, input.Span));

        return new(TypeReaderResult.Fail("The channel was not found."));
    }
}
