using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class GuildRoleTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Role;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options)
    {
        return Task.FromResult((object?)((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Roles![ulong.Parse(value)]);
    }
}
