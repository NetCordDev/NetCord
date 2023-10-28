using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class RoleTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Role;

    public override ValueTask<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return new(((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Roles![Snowflake.Parse(value)]);
    }
}
