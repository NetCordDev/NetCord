namespace NetCord.Services.Commands.TypeReaders;

public class GuildUserTypeReader<TContext> : UserTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is not null)
            return new(GetGuildUser(guild, input.Span));

        return new(TypeReaderResult.Fail("The user was not found."));
    }
}
