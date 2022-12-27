using NetCord;

namespace TimestampTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void Create()
    {
        DateTimeOffset dateTimeOffset = new(2022, 12, 19, 22, 12, 23, default);
        Timestamp timestamp = new(dateTimeOffset);
        Assert.AreEqual(dateTimeOffset, timestamp.DateTime);
        Assert.IsNull(timestamp.Style);
    }

    [TestMethod]
    public void Create2()
    {
        DateTimeOffset dateTimeOffset = new(2022, 12, 19, 22, 12, 23, default);
        var style = TimestampStyle.LongDate;
        Timestamp timestamp = new(dateTimeOffset, style);
        Assert.AreEqual(dateTimeOffset, timestamp.DateTime);
        Assert.AreEqual(style, timestamp.Style);
    }

    [TestMethod]
    public void Parse()
    {
        var values = new long[]
        {
            -62135596800,
            1672169732,
            0,
        };
        foreach (var l in values)
        {
            var valid = $"<t:{l}>";
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(l);
            Assert.IsTrue(Timestamp.TryParse(valid, out var result));
            Assert.AreEqual(dateTimeOffset, result.DateTime);

            result = Timestamp.Parse(valid);
            Assert.AreEqual(dateTimeOffset, result.DateTime);
        }

        foreach (var v in values)
        {
            var valid = $"<t:{v}:R>";
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(v);
            Assert.IsTrue(Timestamp.TryParse(valid, out var result));
            Assert.AreEqual(dateTimeOffset, result.DateTime);
            Assert.AreEqual(TimestampStyle.RelativeTime, result.Style);

            result = Timestamp.Parse(valid);
            Assert.AreEqual(dateTimeOffset, result.DateTime);
            Assert.AreEqual(TimestampStyle.RelativeTime, result.Style);
        }

        var notValid = new string[]
        {
            string.Empty,
            "<t:>",
            "<t:45",
            "45>"
        };
        foreach (var input in notValid)
        {
            Assert.IsFalse(Timestamp.TryParse(input, out _));
            Assert.ThrowsException<FormatException>(() => Timestamp.Parse(input));
        }
    }
}
