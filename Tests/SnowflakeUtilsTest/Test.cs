using NetCord;

namespace SnowflakeTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void Create()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        var id = SnowflakeUtils.Create(dateTime);
        Assert.AreEqual(dateTime, SnowflakeUtils.CreatedAt(id));
        Assert.AreEqual(0, SnowflakeUtils.InternalWorkerId(id));
        Assert.AreEqual(0, SnowflakeUtils.InternalProcessId(id));
        Assert.AreEqual(0, SnowflakeUtils.Increment(id));
    }

    [TestMethod]
    public void Create2()
    {
        DateTimeOffset dateTime = new(2017, 10, 12, 10, 23, 59, default);
        byte worker = 31;
        byte process = 2;
        ushort increment = 2345;
        var id = SnowflakeUtils.Create(dateTime, worker, process, increment);
        Assert.AreEqual(dateTime, SnowflakeUtils.CreatedAt(id));
        Assert.AreEqual(worker, SnowflakeUtils.InternalWorkerId(id));
        Assert.AreEqual(process, SnowflakeUtils.InternalProcessId(id));
        Assert.AreEqual(increment, SnowflakeUtils.Increment(id));
    }
}
