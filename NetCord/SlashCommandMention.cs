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
                return $"</{Name}:{Id}>";
            else
                return $"</{Name} {subCommandName}:{Id}>";
        }
        else
            return $"</{Name} {subCommandGroupName} {subCommandName}:{Id}>";
    }

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var subCommandGroupName = SubCommandGroupName;
        var subCommandName = SubCommandName;
        var name = Name;

        if (subCommandGroupName is null)
        {
            if (subCommandName is null)
            {
                if (destination.Length < 5 + name.Length || !Id.TryFormat(destination[(3 + name.Length)..^1], out var length))
                {
                    charsWritten = 0;
                    return false;
                }

                "</".CopyTo(destination);
                name.CopyTo(destination[2..]);
                destination[2 + name.Length] = ':';
                destination[3 + name.Length + length] = '>';

                charsWritten = 4 + name.Length + length;
                return true;
            }
            else
            {
                if (destination.Length < 6 + name.Length + subCommandName.Length || !Id.TryFormat(destination[(4 + name.Length + subCommandName.Length)..^1], out var length))
                {
                    charsWritten = 0;
                    return false;
                }

                "</".CopyTo(destination);
                name.CopyTo(destination[2..]);
                destination[2 + name.Length] = ' ';
                subCommandName.CopyTo(destination[(3 + name.Length)..]);
                destination[3 + name.Length + subCommandName.Length] = ':';
                destination[4 + name.Length + subCommandName.Length + length] = '>';

                charsWritten = 5 + name.Length + subCommandName.Length + length;
                return true;
            }
        }
        else
        {
            if (destination.Length < 7 + name.Length + subCommandGroupName.Length + subCommandName!.Length || !Id.TryFormat(destination[(5 + name.Length + subCommandGroupName.Length + subCommandName.Length)..^1], out var length))
            {
                charsWritten = 0;
                return false;
            }

            "</".CopyTo(destination);
            name.CopyTo(destination[2..]);
            destination[2 + name.Length] = ' ';
            subCommandGroupName.CopyTo(destination[(3 + name.Length)..]);
            destination[3 + name.Length + subCommandGroupName.Length] = ' ';
            subCommandName.CopyTo(destination[(4 + name.Length + subCommandGroupName.Length)..]);
            destination[4 + name.Length + subCommandGroupName.Length + subCommandName.Length] = ':';
            destination[5 + name.Length + subCommandGroupName.Length + subCommandName.Length + length] = '>';

            charsWritten = 6 + name.Length + subCommandGroupName.Length + subCommandName.Length + length;
            return true;
        }
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
