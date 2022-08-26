using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class GuildUserTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.User;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options)
    {
        var user = ((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Users![new(value)];
        if (user is GuildUser guildUser)
            return Task.FromResult((object?)guildUser);
        else
            throw new InvalidOperationException("The user must be in a guild.");
    }
}
