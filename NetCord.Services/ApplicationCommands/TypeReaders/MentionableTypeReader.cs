namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class MentionableTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Mentionable;

    public override ValueTask<SlashCommandTypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var slashInteraction = (SlashCommandInteraction)context.Interaction;
        var resolvedData = slashInteraction.Data.ResolvedData!;
        var id = Snowflake.Parse(value);

        if (resolvedData.Users is { } users && users.TryGetValue(id, out var user))
            return new(SlashCommandTypeReaderResult.Success(new Mentionable.User(user)));

        return new(SlashCommandTypeReaderResult.Success(new Mentionable.Role(resolvedData.Roles![id])));
    }
}
