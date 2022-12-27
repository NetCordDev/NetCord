using System.Globalization;

using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands.TypeReaders.ChannelTypeReaders;

public class TextChannelTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Channel;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options)
    {
        return Task.FromResult((object?)((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Channels![ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture)]);
    }

    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.TextGuildChannel;
            yield return ChannelType.DMChannel;
            yield return ChannelType.GroupDMChannel;
            yield return ChannelType.AnnouncementGuildChannel;
            yield return ChannelType.AnnouncementGuildThread;
            yield return ChannelType.PublicGuildThread;
            yield return ChannelType.PrivateGuildThread;
        }
    }
}
