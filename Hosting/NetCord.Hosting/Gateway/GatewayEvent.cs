namespace NetCord.Hosting.Gateway;

public partial class GatewayEvent
{
    internal GatewayEvent(GatewayEventId id)
    {
        Id = id;
    }

    internal GatewayEventId Id { get; }
}

public class GatewayEvent<T>
{
    internal GatewayEvent(GatewayEventId id)
    {
        Id = id;
    }

    internal GatewayEventId Id { get; }
}
