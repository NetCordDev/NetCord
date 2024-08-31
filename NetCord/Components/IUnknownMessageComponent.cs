namespace NetCord;

public interface IUnknownMessageComponent : IMessageComponent
{
    public ComponentType Type { get; }
}
