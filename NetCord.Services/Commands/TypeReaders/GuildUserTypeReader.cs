namespace NetCord.Services.Commands.TypeReaders;

public class GuildUserTypeReader<TContext> : UserTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is not null)
            return new(GetGuildUser(guild, input.Span));

        return new(CommandTypeParserResult.Fail("The user was not found."));
    }
}
