using System.Diagnostics.CodeAnalysis;

namespace NetCord
{
    public static class MentionUtils
    {
        public static bool TryParseUser(ReadOnlySpan<char> mention, out DiscordId id)
        {
            if (mention.StartsWith("<@") && mention.EndsWith(">"))
            {
                mention = mention[2..^1];
                if (mention.StartsWith("!"))
                {
                    if (DiscordId.TryCreate(mention[1..].ToString(), out id))
                        return true;
                }
                else if (DiscordId.TryCreate(mention.ToString(), out id))
                    return true;
            }
            id = default;
            return false;
        }

        public static DiscordId ParseUser(ReadOnlySpan<char> mention)
        {
            if (TryParseUser(mention, out DiscordId id))
                return id;
            else
                throw new FormatException("Cannot parse the mention");
        }

        public static bool TryParseChannel(ReadOnlySpan<char> mention, out DiscordId id)
        {
            if (mention.StartsWith("<#") && mention.EndsWith(">"))
            {
                if (DiscordId.TryCreate(mention[2..^1].ToString(), out id))
                    return true;
            }
            id = default;
            return false;
        }

        public static DiscordId ParseChannel(ReadOnlySpan<char> mention)
        {
            if (TryParseChannel(mention, out DiscordId id))
                return id;
            else
                throw new FormatException("Cannot parse the mention");
        }

        public static bool TryParseRole(ReadOnlySpan<char> mention, [NotNullWhen(true)] out DiscordId id)
        {
            if (mention.StartsWith("<@&") && mention.EndsWith(">"))
            {
                if (DiscordId.TryCreate(mention[3..^1].ToString(), out id))
                    return true;
            }
            id = default;
            return false;
        }

        public static DiscordId ParseRole(ReadOnlySpan<char> mention)
        {
            if (TryParseChannel(mention, out DiscordId id))
                return id;
            else
                throw new FormatException("Cannot parse the mention");
        }
    }
}
