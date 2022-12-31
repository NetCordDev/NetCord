using System.Globalization;

using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands.TypeReaders.ChannelTypeReaders;

public class DMChannelTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Channel;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        return Task.FromResult<object?>(((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Channels![ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture)]);
    }

    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.DMChannel;
        }
    }
}
