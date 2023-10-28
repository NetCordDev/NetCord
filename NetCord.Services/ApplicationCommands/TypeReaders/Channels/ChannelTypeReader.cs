using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class ChannelTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Channel;

    public override ValueTask<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return new(((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Channels![Snowflake.Parse(value)]);
    }
}
