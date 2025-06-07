namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class EntryPointCommandAttribute(string name, string description) : ApplicationCommandAttribute(name)
{
    public string Description { get; } = description;
}
