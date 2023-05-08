using System.Globalization;

using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class ChannelTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Channel;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return Task.FromResult<object?>(((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Channels![ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture)]);
    }
}
