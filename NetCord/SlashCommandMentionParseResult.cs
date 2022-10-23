namespace NetCord;

public class SlashCommandMentionParseResult : Entity
{
    public override ulong Id { get; }

    public string Name { get; }

    public string? SubCommandGroupName { get; }

    public string? SubCommandName { get; }

    public SlashCommandMentionParseResult(ulong id, string name, string? subCommandGroupName, string? subCommandName)
    {
        Id = id;
        Name = name;
        SubCommandGroupName = subCommandGroupName;
        SubCommandName = subCommandName;
    }
}
