using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class RoleTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Role;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return Task.FromResult<object?>(((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Roles![ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture)]);
    }
}
