using System.Diagnostics.CodeAnalysis;

namespace NetCord
{
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
                throw new FormatException("Cannot parse the mention");
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
                throw new FormatException("Cannot parse the mention");
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
            if (TryParseChannel(mention, out Snowflake id))
                return id;
            else
                throw new FormatException("Cannot parse the mention");
        }
    }
}
