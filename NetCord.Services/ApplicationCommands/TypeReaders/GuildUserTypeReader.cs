using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class GuildUserTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.User;

    public override ValueTask<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var user = ((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Users![ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture)];
        if (user is GuildUser guildUser)
            return new(guildUser);
        else
            throw new InvalidOperationException("The user must be in a guild.");
    }
}
