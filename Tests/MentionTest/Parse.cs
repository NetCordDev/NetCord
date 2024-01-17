using NetCord;

namespace MentionTest;

[TestClass]
public unsafe class Parse
{
    private const ulong Id = 859359702012788756;

    [TestMethod]
    public void User()
    {
        ParseTest(&Mention.ParseUser, Id, id => $"<@{id}>");
        ParseTest(&Mention.ParseUser, Id, id => $"<@!{id}>");
        TryParseTest(&Mention.TryParseUser, Id, id => $"<@{id}>");
        TryParseTest(&Mention.TryParseUser, Id, id => $"<@!{id}>");
    }

    [TestMethod]
    public void Channel()
    {
        ParseTest(&Mention.ParseChannel, Id, id => $"<#{id}>");
        TryParseTest(&Mention.TryParseChannel, Id, id => $"<#{id}>");
    }

    [TestMethod]
    public void Role()
    {
        ParseTest(&Mention.ParseRole, Id, id => $"<@&{id}>");
        TryParseTest(&Mention.TryParseRole, Id, id => $"<@&{id}>");
    }

    [TestMethod]
    public void SlashCommand()
    {
        var testMentions = new SlashCommandMention[]
        {
            new(Id, "name"),
            new(Id, "name", "name2"),
            new(Id, "name", "name2", "name3")
        };

        foreach (var mention in testMentions)
        {
            ParseTest(&Mention.ParseSlashCommand, mention, m => m.ToString());
            TryParseTest(&Mention.TryParseSlashCommand, mention, m => m.ToString());
        }
    }

    [TestMethod]
    public void Timestamp()
    {
        DateTimeOffset dateTimeOffset = new(2021, 07, 01, 00, 00, 00, TimeSpan.Zero);
        var testMentions = new Timestamp[]
        {
            new(dateTimeOffset, TimestampStyle.ShortTime),
            new(dateTimeOffset, TimestampStyle.LongTime),
            new(dateTimeOffset, TimestampStyle.ShortDate),
            new(dateTimeOffset, TimestampStyle.LongDate),
            new(dateTimeOffset, TimestampStyle.ShortDateTime),
            new(dateTimeOffset, TimestampStyle.LongDateTime),
            new(dateTimeOffset, TimestampStyle.RelativeTime),
        };

        foreach (var mention in testMentions)
        {
            ParseTest(&Mention.ParseTimestamp, mention, m => m.ToString());
            TryParseTest(&Mention.TryParseTimestamp, mention, m => m.ToString());
        }
    }

    [TestMethod]
    public void GuildNavigation()
    {
        var testMentions = new GuildNavigation[]
        {
            new(GuildNavigationType.Customize),
            new(GuildNavigationType.Browse),
            new(GuildNavigationType.Guide),
        };

        foreach (var mention in testMentions)
        {
            ParseTest(&Mention.ParseGuildNavigation, mention, m => m.ToString());
            TryParseTest(&Mention.TryParseGuildNavigation, mention, m => m.ToString());
        }
    }

    public static void ParseTest<T>(delegate*<ReadOnlySpan<char>, T> parse, T expected, Func<T, string> validMentionFunc)
    {
        var mention = validMentionFunc(expected);

        Assert.AreEqual(expected, parse(mention));

        var badMentions = new string[]
        {
            string.Empty,
            mention[..^1],
            mention[1..],
        };

        foreach (var badMention in badMentions)
            Assert.ThrowsException<FormatException>(() => parse(badMention));
    }

    public static void TryParseTest<T>(delegate*<ReadOnlySpan<char>, out T?, bool> tryParse, T expected, Func<T, string> validMentionFunc)
    {
        var mention = validMentionFunc(expected);

        Assert.IsTrue(tryParse(mention, out var result));
        Assert.AreEqual(expected, result);

        var badMentions = new string[]
        {
            string.Empty,
            mention[..^1],
            mention[1..],
        };

        foreach (var badMention in badMentions)
            Assert.IsFalse(tryParse(badMention, out _));
    }
}
