namespace NetCord.Services.SlashCommands.TypeReaders;

public class GuildRoleTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Role;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options)
    {
        return Task.FromResult((object?)context.Interaction.Data.ResolvedData!.Roles![new(value)]);
    }
}