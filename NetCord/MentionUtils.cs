using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NetCord;

public static class MentionUtils
{
    public static bool TryParseUser(ReadOnlySpan<char> mention, out ulong id)
    {
        if (mention.StartsWith("<@") && mention.EndsWith(">"))
        {
            mention = mention[2..^1];
            if (ulong.TryParse(mention.StartsWith("!") ? mention[1..] : mention, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                return true;
        }
        else
            id = default;

        return false;
    }

    public static ulong ParseUser(ReadOnlySpan<char> mention)
    {
        if (TryParseUser(mention, out ulong id))
            return id;
        else
            throw new FormatException("Cannot parse the mention.");
    }

    public static bool TryParseChannel(ReadOnlySpan<char> mention, out ulong id)
    {
        if (mention.StartsWith("<#") && mention.EndsWith(">"))
        {
            if (ulong.TryParse(mention[2..^1], NumberStyles.None, CultureInfo.InvariantCulture, out id))
                return true;
        }
        else
            id = default;

        return false;
    }

    public static ulong ParseChannel(ReadOnlySpan<char> mention)
    {
        if (TryParseChannel(mention, out ulong id))
            return id;
        else
            throw new FormatException("Cannot parse the mention.");
    }

    public static bool TryParseRole(ReadOnlySpan<char> mention, out ulong id)
    {
        if (mention.StartsWith("<@&") && mention.EndsWith(">"))
        {
            if (ulong.TryParse(mention[3..^1], NumberStyles.None, CultureInfo.InvariantCulture, out id))
                return true;
        }
        else
            id = default;

        return false;
    }

    public static ulong ParseRole(ReadOnlySpan<char> mention)
    {
        if (TryParseRole(mention, out ulong id))
            return id;
        else
            throw new FormatException("Cannot parse the mention.");
    }

    public static bool TryParseSlashCommand(ReadOnlySpan<char> mention, [MaybeNullWhen(false)] out SlashCommandMention result)
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
            if (!ulong.TryParse(mention[(index + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out var id))
                goto Fail;

            result = i switch
            {
                0 => new(id, s[0]),
                1 => new(id, s[0], s[1]),
                _ => new(id, s[0], s[1], s[2]),
            };
            return true;
        }

        Fail:
        result = null;
        return false;
    }

    public static SlashCommandMention ParseSlashCommand(ReadOnlySpan<char> mention)
    {
        if (TryParseSlashCommand(mention, out var result))
            return result;
        else
            throw new FormatException("Cannot parse the mention.");
    }

    public static bool TryParseTimestamp(ReadOnlySpan<char> mention, out Timestamp result)
    {
        return Timestamp.TryParse(mention, out result);
    }

    public static Timestamp ParseTimestamp(ReadOnlySpan<char> mention)
    {
        if (TryParseTimestamp(mention, out var result))
            return result;
        else
            throw new FormatException("Cannot parse the mention.");
    }

    public static bool TryParseGuildNavigation(ReadOnlySpan<char> mention, out GuildNavigation result)
    {
        return GuildNavigation.TryParse(mention, out result);
    }

    public static GuildNavigation ParseGuildNavigation(ReadOnlySpan<char> mention)
    {
        if (TryParseGuildNavigation(mention, out var result))
            return result;
        else
            throw new FormatException("Cannot parse the mention.");
    }
}
