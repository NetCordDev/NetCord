namespace NetCord.Hosting.Gateway;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GatewayEventAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
