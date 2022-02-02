namespace NetCord.Services.SlashCommands.TypeReaders;

public class MentionableTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : BaseSlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Mentionable;

    public override Task<object> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options)
    {
        var roles = context.Interaction.Data.ResolvedData!.Roles;
        DiscordId id = new(value);
        if (roles != null && roles.TryGetValue(id, out var role))
            return Task.FromResult((object)new Mentionable(role));
        else
            return Task.FromResult((object)new Mentionable(context.Interaction.Data.ResolvedData!.Users![id]));
    }
}
