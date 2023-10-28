using NetCord;

namespace SnowflakeTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void Create()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        var id = Snowflake.Create(dateTime);
        Assert.AreEqual(dateTime, Snowflake.CreatedAt(id));
        Assert.AreEqual(0, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(0, Snowflake.InternalProcessId(id));
        Assert.AreEqual(0, Snowflake.Increment(id));
    }

    [TestMethod]
    public void Create2()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        byte worker = 31;
        byte process = 2;
        ushort increment = 2345;
        var id = Snowflake.Create(dateTime, worker, process, increment);
        Assert.AreEqual(dateTime, Snowflake.CreatedAt(id));
        Assert.AreEqual(worker, Snowflake.InternalWorkerId(id));
        Assert.AreEqual(process, Snowflake.InternalProcessId(id));
        Assert.AreEqual(increment, Snowflake.Increment(id));
    }

    [TestMethod]
    public void Parse()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        var id = Snowflake.Create(dateTime);
        Assert.IsTrue(Snowflake.TryParse(id.ToString(), out ulong result));
        Assert.AreEqual(id, result);
    }

    [TestMethod]
    public void Parse2()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        byte worker = 31;
        byte process = 2;
        ushort increment = 2345;
        var id = Snowflake.Create(dateTime, worker, process, increment);
        Assert.IsTrue(Snowflake.TryParse(id.ToString(), out ulong result));
        Assert.AreEqual(id, result);
    }
}
