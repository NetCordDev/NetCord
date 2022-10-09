using NetCord;

namespace MentionUtilsTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void User()
    {
        ParseTest(MentionUtils.ParseUser, id => $"<@{id}>");
        ParseTest(MentionUtils.ParseUser, id => $"<@!{id}>");
        TryParseTest(MentionUtils.TryParseUser, id => $"<@{id}>");
        TryParseTest(MentionUtils.TryParseUser, id => $"<@!{id}>");
    }

    [TestMethod]
    public void Channel()
    {
        ParseTest(MentionUtils.ParseChannel, id => $"<#{id}>");
        TryParseTest(MentionUtils.TryParseChannel, id => $"<#{id}>");
    }

    [TestMethod]
    public void Role()
    {
        ParseTest(MentionUtils.ParseRole, id => $"<@&{id}>");
        TryParseTest(MentionUtils.TryParseRole, id => $"<@&{id}>");
    }

    [TestMethod]
    public void SlashCommand()
    {
        Snowflake id = 859359702012788756;

        var result = MentionUtils.ParseSlashCommand($"</name:{id}>");
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("name", result.Name);
        Assert.AreEqual(null, result.SubCommandGroupName);
        Assert.AreEqual(null, result.SubCommandName);

        result = MentionUtils.ParseSlashCommand($"</name name2:{id}>");
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("name", result.Name);
        Assert.AreEqual(null, result.SubCommandGroupName);
        Assert.AreEqual("name2", result.SubCommandName);

        result = MentionUtils.ParseSlashCommand($"</name name2 name3:{id}>");
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("name", result.Name);
        Assert.AreEqual("name2", result.SubCommandGroupName);
        Assert.AreEqual("name3", result.SubCommandName);

        var badMentions = new string[]
        {
            string.Empty,
            $"</name name2 name3 name4:{id}>",
        };

        foreach (var badMention in badMentions)
            Assert.ThrowsException<FormatException>(() => MentionUtils.ParseSlashCommand(badMention));

        Assert.IsTrue(MentionUtils.TryParseSlashCommand($"</name:{id}>", out result));
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("name", result.Name);
        Assert.AreEqual(null, result.SubCommandGroupName);
        Assert.AreEqual(null, result.SubCommandName);

        Assert.IsTrue(MentionUtils.TryParseSlashCommand($"</name name2:{id}>", out result));
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("name", result.Name);
        Assert.AreEqual(null, result.SubCommandGroupName);
        Assert.AreEqual("name2", result.SubCommandName);

        Assert.IsTrue(MentionUtils.TryParseSlashCommand($"</name name2 name3:{id}>", out result));
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("name", result.Name);
        Assert.AreEqual("name2", result.SubCommandGroupName);
        Assert.AreEqual("name3", result.SubCommandName);

        foreach (var badMention in badMentions)
            Assert.IsFalse(MentionUtils.TryParseChannel(badMention, out _));
    }

    public static void ParseTest(ParseDelegate del, Func<Snowflake, string> validMentionFunc)
    {
        Snowflake id = 859359702012788756;
        var mention = validMentionFunc(id);

        Assert.AreEqual(id, del(mention));

        var badMentions = new string[]
        {
            string.Empty,
            mention[..^1],
            mention[1..],
        };

        foreach (var badMention in badMentions)
            Assert.ThrowsException<FormatException>(() => MentionUtils.ParseChannel(badMention));
    }

    public static void TryParseTest(TryParseDelegate del, Func<Snowflake, string> validMentionFunc)
    {
        Snowflake id = 859359702012788756;
        var mention = validMentionFunc(id);

        Assert.IsTrue(del(mention, out var result));
        Assert.AreEqual(id, result);

        var badMentions = new string[]
        {
            string.Empty,
            mention[..^1],
            mention[1..],
        };

        foreach (var badMention in badMentions)
            Assert.IsFalse(MentionUtils.TryParseChannel(badMention, out _));
    }

    public delegate Snowflake ParseDelegate(ReadOnlySpan<char> span);
    public delegate bool TryParseDelegate(ReadOnlySpan<char> span, out Snowflake result);
}
