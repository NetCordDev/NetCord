using System.Text;

using NetCord;

namespace SnowflakeTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void Create1()
    {
        TestCreate1Success(new(2017, 10, 12, 10, 23, 59, default));

        TestCreate1Success(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch));

        TestCreate1Success(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch + (1L << 42) - 1));

        TestCreate1Fail(DateTimeOffset.MinValue);

        TestCreate1Fail(DateTimeOffset.MaxValue);

        TestCreate1Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch - 1));

        TestCreate1Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch + (1L << 42) - 1 + 1));
    }

    private static void TestCreate1Success(DateTimeOffset dateTimeOffset)
    {
        var id = Snowflake.Create(dateTimeOffset);
        Assert.AreEqual(dateTimeOffset, Snowflake.Timestamp(id));
        Assert.AreEqual(0, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(0, Snowflake.InternalProcessId(id));
        Assert.AreEqual(0, Snowflake.Increment(id));

        id = Snowflake.Create(dateTimeOffset.ToUnixTimeMilliseconds());
        Assert.AreEqual(dateTimeOffset, Snowflake.Timestamp(id));
        Assert.AreEqual(0, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(0, Snowflake.InternalProcessId(id));
        Assert.AreEqual(0, Snowflake.Increment(id));

        id = Snowflake.CreateUnsafe(dateTimeOffset);
        Assert.AreEqual(dateTimeOffset, Snowflake.Timestamp(id));
        Assert.AreEqual(0, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(0, Snowflake.InternalProcessId(id));
        Assert.AreEqual(0, Snowflake.Increment(id));

        id = Snowflake.CreateUnsafe(dateTimeOffset.ToUnixTimeMilliseconds());
        Assert.AreEqual(dateTimeOffset, Snowflake.Timestamp(id));
        Assert.AreEqual(0, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(0, Snowflake.InternalProcessId(id));
        Assert.AreEqual(0, Snowflake.Increment(id));
    }

    private static void TestCreate1Fail(DateTimeOffset dateTimeOffset)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Snowflake.Create(dateTimeOffset));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Snowflake.Create(dateTimeOffset.ToUnixTimeMilliseconds()));

        // Unsafe methods should not throw exceptions for out of range values
        _ = Snowflake.CreateUnsafe(dateTimeOffset);

        _ = Snowflake.CreateUnsafe(dateTimeOffset.ToUnixTimeMilliseconds());
    }

    [TestMethod]
    public void Create2()
    {
        TestCreate2Success(new(2017, 10, 12, 10, 23, 59, default), 31, 2, 2345);

        TestCreate2Success(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch), 0, 0, 0);

        TestCreate2Success(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch + (1L << 42) - 1), (1 << 5) - 1, (1 << 5) - 1, (1 << 12) - 1);

        TestCreate2Fail(DateTimeOffset.MinValue, 0, 0, 0);
        TestCreate2Fail(DateTimeOffset.MaxValue, 0, 0, 0);
        TestCreate2Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch - 1), 0, 0, 0);
        TestCreate2Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch + (1L << 42) - 1 + 1), 0, 0, 0);

        TestCreate2Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch), byte.MaxValue, 0, 0);
        TestCreate2Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch), (1 << 5) - 1 + 1, 0, 0);

        TestCreate2Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch), 0, byte.MaxValue, 0);
        TestCreate2Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch), 0, (1 << 5) - 1 + 1, 0);

        TestCreate2Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch), 0, 0, ushort.MaxValue);
        TestCreate2Fail(DateTimeOffset.FromUnixTimeMilliseconds(Discord.Epoch), 0, 0, (1 << 12) - 1 + 1);
    }

    private static void TestCreate2Success(DateTimeOffset dateTimeOffset, byte worker, byte process, ushort increment)
    {
        var id = Snowflake.Create(dateTimeOffset, worker, process, increment);
        Assert.AreEqual(dateTimeOffset, Snowflake.Timestamp(id));
        Assert.AreEqual(worker, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(process, Snowflake.InternalProcessId(id));
        Assert.AreEqual(increment, Snowflake.Increment(id));

        id = Snowflake.Create(dateTimeOffset.ToUnixTimeMilliseconds(), worker, process, increment);
        Assert.AreEqual(dateTimeOffset, Snowflake.Timestamp(id));
        Assert.AreEqual(worker, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(process, Snowflake.InternalProcessId(id));
        Assert.AreEqual(increment, Snowflake.Increment(id));

        id = Snowflake.CreateUnsafe(dateTimeOffset, worker, process, increment);
        Assert.AreEqual(dateTimeOffset, Snowflake.Timestamp(id));
        Assert.AreEqual(worker, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(process, Snowflake.InternalProcessId(id));
        Assert.AreEqual(increment, Snowflake.Increment(id));

        id = Snowflake.CreateUnsafe(dateTimeOffset.ToUnixTimeMilliseconds(), worker, process, increment);
        Assert.AreEqual(dateTimeOffset, Snowflake.Timestamp(id));
        Assert.AreEqual(worker, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(process, Snowflake.InternalProcessId(id));
        Assert.AreEqual(increment, Snowflake.Increment(id));
    }

    private static void TestCreate2Fail(DateTimeOffset dateTimeOffset, byte worker, byte process, ushort increment)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Snowflake.Create(dateTimeOffset, worker, process, increment));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Snowflake.Create(dateTimeOffset.ToUnixTimeMilliseconds(), worker, process, increment));

        // Unsafe methods should not throw exceptions for out of range values
        _ = Snowflake.CreateUnsafe(dateTimeOffset, worker, process, increment);

        _ = Snowflake.CreateUnsafe(dateTimeOffset.ToUnixTimeMilliseconds(), worker, process, increment);
    }

    [TestMethod]
    public void Parse()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        var id = Snowflake.Create(dateTime);
        var stringId = id.ToString();

        Assert.IsTrue(Snowflake.TryParse(stringId, out ulong result));
        Assert.AreEqual(id, result);

        Assert.IsTrue(Snowflake.TryParse(Encoding.UTF8.GetBytes(stringId), out var result2));
        Assert.AreEqual(id, result2);
    }

    [TestMethod]
    public void Parse2()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        byte worker = 31;
        byte process = 2;
        ushort increment = 2345;
        var id = Snowflake.Create(dateTime, worker, process, increment);
        var stringId = id.ToString();

        Assert.IsTrue(Snowflake.TryParse(stringId, out ulong result));
        Assert.AreEqual(id, result);

        Assert.IsTrue(Snowflake.TryParse(Encoding.UTF8.GetBytes(stringId), out var result2));
        Assert.AreEqual(id, result2);
    }
}
