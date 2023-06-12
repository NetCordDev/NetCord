using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class UserTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.User;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return Task.FromResult<object?>(((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Users![ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture)]);
    }
}
