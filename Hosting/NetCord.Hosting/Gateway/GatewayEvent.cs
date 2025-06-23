namespace NetCord.Hosting.Gateway;

public partial class GatewayEvent
{
    internal GatewayEvent(string name)
    {
        Name = name;
    }

    internal string Name { get; }
}

public class GatewayEvent<T>
{
    internal GatewayEvent(string name)
    {
        Name = name;
    }

    internal string Name { get; }
}
