namespace NetCord;

public class SlashCommandMention(ulong id, string name) : Entity, IEquatable<SlashCommandMention>
{
    public override ulong Id { get; } = id;

    public string Name { get; } = name;

    public string? SubCommandGroupName { get; }

    public string? SubCommandName { get; }

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
                return Mention.ApplicationCommandToString(Name, Id);
            else
                return Mention.ApplicationCommandToString(Name, subCommandName, Id);
        }
        else
            return Mention.ApplicationCommandToString(Name, subCommandGroupName, subCommandName!, Id);
    }

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var subCommandGroupName = SubCommandGroupName;
        var subCommandName = SubCommandName;

        if (subCommandGroupName is null)
        {
            if (subCommandName is null)
                return Mention.TryFormatApplicationCommand(destination, out charsWritten, Id, Name);
            else
                return Mention.TryFormatApplicationCommand(destination, out charsWritten, Id, Name, subCommandName);
        }
        else
            return Mention.TryFormatApplicationCommand(destination, out charsWritten, Id, Name, subCommandGroupName, subCommandName!);
    }

    public static bool operator ==(SlashCommandMention? left, SlashCommandMention? right) => left is null ? right is null : Equals(left, right);

    public static bool operator !=(SlashCommandMention? left, SlashCommandMention? right) => !(left == right);

    public override bool Equals(object? obj) => obj is SlashCommandMention mention && EqualsHelper(mention);

    public bool Equals(SlashCommandMention? other) => other is not null && EqualsHelper(other);

    private bool EqualsHelper(SlashCommandMention other) => Id == other.Id
                                                            && Name == other.Name
                                                            && SubCommandGroupName == other.SubCommandGroupName
                                                            && SubCommandName == other.SubCommandName;

    public override int GetHashCode() => HashCode.Combine(Id, Name, SubCommandGroupName, SubCommandName);
}
