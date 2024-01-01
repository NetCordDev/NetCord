namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class GuildUserTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.User;

    public override ValueTask<TypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var user = ((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Users![Snowflake.Parse(value)];
        return new(user is GuildUser guildUser
            ? TypeReaderResult.Success(guildUser)
            : TypeReaderResult.Fail("The user must be in the guild."));
    }
}
