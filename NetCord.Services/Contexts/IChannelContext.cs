namespace NetCord.Services;

public interface IChannelContext : IContext
{
    public TextChannel? Channel { get; }
}