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
        Assert.AreEqual(dateTime, id.CreatedAt);
        Assert.AreEqual(0, id.InternalWorkerId);
        Assert.AreEqual(0, id.InternalProcessId);
        Assert.AreEqual(0, id.Increment);
    }

    [TestMethod]
    public void Create2()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        byte worker = 31;
        byte process = 2;
        ushort increment = 2345;
        var id = Snowflake.Create(dateTime, worker, process, increment);
        Assert.AreEqual(dateTime, id.CreatedAt);
        Assert.AreEqual(worker, id.InternalWorkerId);
        Assert.AreEqual(process, id.InternalProcessId);
        Assert.AreEqual(increment, id.Increment);
    }
}