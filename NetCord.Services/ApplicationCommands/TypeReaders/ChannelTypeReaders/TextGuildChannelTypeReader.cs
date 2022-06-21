using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands.TypeReaders.ChannelTypeReaders;

public class TextGuildChannelTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Channel;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options)
    {
        return Task.FromResult((object?)((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Channels![new(value)]);
    }

    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.TextGuildChannel;
        }
    }
}