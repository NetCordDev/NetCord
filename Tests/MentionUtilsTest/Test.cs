using System.Diagnostics.CodeAnalysis;

using NetCord;

namespace MentionUtilsTest;

[TestClass]
public unsafe class Test
{
    private const ulong Id = 859359702012788756;

    [TestMethod]
    public void User()
    {
        ParseTest(&MentionUtils.ParseUser, Id, id => $"<@{id}>");
        ParseTest(&MentionUtils.ParseUser, Id, id => $"<@!{id}>");
        TryParseTest(&MentionUtils.TryParseUser, Id, id => $"<@{id}>");
        TryParseTest(&MentionUtils.TryParseUser, Id, id => $"<@!{id}>");
    }

    [TestMethod]
    public void Channel()
    {
        ParseTest(&MentionUtils.ParseChannel, Id, id => $"<#{id}>");
        TryParseTest(&MentionUtils.TryParseChannel, Id, id => $"<#{id}>");
    }

    [TestMethod]
    public void Role()
    {
        ParseTest(&MentionUtils.ParseRole, Id, id => $"<@&{id}>");
        TryParseTest(&MentionUtils.TryParseRole, Id, id => $"<@&{id}>");
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
            ParseTest(&MentionUtils.ParseSlashCommand, mention, m => m.ToString());
            TryParseTest(&MentionUtils.TryParseSlashCommand, mention, m => m.ToString());
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
            ParseTest(&MentionUtils.ParseTimestamp, mention, m => m.ToString());
            TryParseTest(&MentionUtils.TryParseTimestamp, mention, m => m.ToString());
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
            ParseTest(&MentionUtils.ParseGuildNavigation, mention, m => m.ToString());
            TryParseTest(&MentionUtils.TryParseGuildNavigation, mention, m => m.ToString());
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
