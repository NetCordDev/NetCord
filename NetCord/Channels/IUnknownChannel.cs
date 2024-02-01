namespace NetCord;

public interface IUnknownChannel : IEntity, ISpanFormattable
{
    public ChannelType Type { get; }
}
