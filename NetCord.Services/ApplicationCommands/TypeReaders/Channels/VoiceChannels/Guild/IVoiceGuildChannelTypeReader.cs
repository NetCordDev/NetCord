namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class IVoiceGuildChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.VoiceGuildChannel;
            yield return ChannelType.StageGuildChannel;
        }
    }
}
