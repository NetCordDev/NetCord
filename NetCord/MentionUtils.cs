using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public static class MentionUtils
{
    public static bool TryParseUser(ReadOnlySpan<char> mention, out Snowflake id)
    {
        if (mention.StartsWith("<@") && mention.EndsWith(">"))
        {
            mention = mention[2..^1];
            if (mention.StartsWith("!"))
            {
                if (Snowflake.TryCreate(mention[1..].ToString(), out id))
                    return true;
            }
            else if (Snowflake.TryCreate(mention.ToString(), out id))
                return true;
        }
        id = default;
        return false;
    }

    public static Snowflake ParseUser(ReadOnlySpan<char> mention)
    {
        if (TryParseUser(mention, out Snowflake id))
            return id;
        else
            throw new FormatException("Cannot parse the mention.");
    }

    public static bool TryParseChannel(ReadOnlySpan<char> mention, out Snowflake id)
    {
        if (mention.StartsWith("<#") && mention.EndsWith(">"))
        {
            if (Snowflake.TryCreate(mention[2..^1].ToString(), out id))
                return true;
        }
        id = default;
        return false;
    }

    public static Snowflake ParseChannel(ReadOnlySpan<char> mention)
    {
        if (TryParseChannel(mention, out Snowflake id))
            return id;
        else
            throw new FormatException("Cannot parse the mention.");
    }

    public static bool TryParseRole(ReadOnlySpan<char> mention, [NotNullWhen(true)] out Snowflake id)
    {
        if (mention.StartsWith("<@&") && mention.EndsWith(">"))
        {
            if (Snowflake.TryCreate(mention[3..^1].ToString(), out id))
                return true;
        }
        id = default;
        return false;
    }

    public static Snowflake ParseRole(ReadOnlySpan<char> mention)
    {
        if (TryParseRole(mention, out Snowflake id))
            return id;
        else
            throw new FormatException("Cannot parse the mention.");
    }

    public static bool TryParseSlashCommand(ReadOnlySpan<char> mention, [NotNullWhen(true)] out SlashCommandMentionParseResult? result)
    {
        if (mention.StartsWith("</") && mention.EndsWith(">"))
        {
            mention = mention[2..^1];
            int index = mention.IndexOf(':');
            if (index == -1)
                goto Fail;

            var names = mention[..index];
            var s = new string[3];
            int i = 0;
            while (i < 3)
            {
                int x = names.IndexOf(' ');
                if (x == -1)
                {
                    s[i] = names.ToString();
                    goto Skip;
                }
                else
                {
                    s[i] = names[..x].ToString();
                    names = names[(x + 1)..];
                }
                i++;
            }

            if (!names.IsEmpty)
                goto Fail;

            Skip:

            if (!Snowflake.TryCreate(mention[(index + 1)..].ToString(), out var id))
                goto Fail;

            result = i switch
            {
                0 => new(id, s[0], null, null),
                1 => new(id, s[0], null, s[1]),
                _ => new(id, s[0], s[1], s[2]),
            };
            return true;
        }

        Fail:
        result = null;
        return false;
    }

    public static SlashCommandMentionParseResult ParseSlashCommand(ReadOnlySpan<char> mention)
    {
        if (TryParseSlashCommand(mention, out var result))
            return result;
        else
            throw new FormatException("Cannot parse the mention.");
    }
}
