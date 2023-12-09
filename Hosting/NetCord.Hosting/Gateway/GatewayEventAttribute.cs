namespace NetCord.Hosting.Gateway;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GatewayEventAttribute : Attribute
{
    public GatewayEventAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
