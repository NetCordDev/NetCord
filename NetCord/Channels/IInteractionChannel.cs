namespace NetCord;

public interface IInteractionChannel : IEntity
{
    public Permissions Permissions { get; }

    public string ToString();
}
