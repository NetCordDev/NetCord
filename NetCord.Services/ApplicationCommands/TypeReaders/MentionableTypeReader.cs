namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class MentionableTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Mentionable;

    public override ValueTask<TypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var slashInteraction = (SlashCommandInteraction)context.Interaction;
        var resolvedData = slashInteraction.Data.ResolvedData!;
        var roles = resolvedData.Roles;
        var id = Snowflake.Parse(value);
        if (roles is not null && roles.TryGetValue(id, out var role))
            return new(TypeReaderResult.Success(new Mentionable(role)));
        else
            return new(TypeReaderResult.Success(new Mentionable(resolvedData.Users![id])));
    }
}
