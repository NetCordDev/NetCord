using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class EntryPointCommandAttribute(string name, string description, EntryPointCommandHandler handler) : ApplicationCommandAttribute(name)
{
    public string Description { get; } = description;

    public EntryPointCommandHandler Handler { get; } = handler;
}
