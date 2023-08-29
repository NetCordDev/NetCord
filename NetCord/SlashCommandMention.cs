namespace NetCord;

public class SlashCommandMention : Entity
{
    public override ulong Id { get; }

    public string Name { get; }

    public string? SubCommandGroupName { get; }

    public string? SubCommandName { get; }

    public SlashCommandMention(ulong id, string name)
    {
        Id = id;
        Name = name;
    }

    public SlashCommandMention(ulong id, string name, string subCommandName) : this(id, name)
    {
        SubCommandName = subCommandName;
    }

    public SlashCommandMention(ulong id, string name, string subCommandGroupName, string subCommandName) : this(id, name, subCommandName)
    {
        SubCommandGroupName = subCommandGroupName;
    }

    public override string ToString()
    {
        var subCommandGroupName = SubCommandGroupName;
        var subCommandName = SubCommandName;
        if (subCommandGroupName is null)
        {
            if (subCommandName is null)
                return $"</{Name}:{Id}>";
            else
                return $"</{Name} {subCommandName}:{Id}>";
        }
        else
            return $"</{Name} {subCommandGroupName} {subCommandName}:{Id}>";
    }

    public static bool operator ==(SlashCommandMention left, SlashCommandMention right)
        => left.Id == right.Id && left.Name == right.Name && left.SubCommandGroupName == right.SubCommandGroupName && left.SubCommandName == right.SubCommandName;

    public static bool operator !=(SlashCommandMention left, SlashCommandMention right) => !(left == right);

    public override bool Equals(object? obj) => obj is SlashCommandMention mention && this == mention;

    public override int GetHashCode() => HashCode.Combine(Id, Name, SubCommandGroupName, SubCommandName);
}
