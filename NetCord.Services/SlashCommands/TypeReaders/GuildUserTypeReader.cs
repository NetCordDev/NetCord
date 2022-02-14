namespace NetCord.Services.SlashCommands.TypeReaders;

public class GuildUserTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.User;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options)
    {
        var user = context.Interaction.Data.ResolvedData!.Users![new(value)];
        if (user is GuildUser guildUser)
            return Task.FromResult((object?)guildUser);
        else
            throw new InvalidOperationException("The user must be in a guild");
    }
}