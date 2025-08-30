namespace NetCord.Services.Commands;

[GenerateMethodsForProperties]
public partial class CommandBuilder(IEnumerable<string> aliases, Delegate handler)
{
    public IEnumerable<string> Aliases => aliases;

    public Delegate Handler => handler;

    public int Priority { get; set; }
}
