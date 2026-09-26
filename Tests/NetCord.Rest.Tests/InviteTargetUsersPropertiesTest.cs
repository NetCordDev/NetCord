namespace NetCord.Rest.Tests;

[TestClass]
public class InviteTargetUsersPropertiesTest(TestContext context)
{
    public static IEnumerable<object[]> FromEnumerableSerializationData()
    {
        var baseSnowflake = Snowflake.Create(new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero), 31, 12, 2332);

        object[] Convert(IEnumerable<ulong> userIds) => [userIds.Select(u => u + baseSnowflake)];

        yield return Convert([]);
        yield return Convert([1]);
        yield return Convert([1, 2, 3]);
        yield return Convert(Enumerable.Range(0, 100).Select(i => (ulong)i));
        yield return Convert(Enumerable.Range(0, 100000).Select(i => (ulong)i));
        yield return Convert(Enumerable.Range(0, 1000000).Select(i => (ulong)i));
    }

    [TestMethod]
    [DynamicData(nameof(FromEnumerableSerializationData))]
    public async Task FromEnumerableSerializationAsync(IEnumerable<ulong> userIds)
    {
        var properties = InviteTargetUsersProperties.FromEnumerable(userIds);
        using var content = ((IHttpSerializable)properties).Serialize();

        var output = await content.ReadAsStringAsync(context.CancellationToken).ConfigureAwait(false);

        var split = output.Split("\r\n");

        Assert.AreEqual(split[^1], string.Empty);

        CollectionAssert.AreEqual(split.SkipLast(1).Select(l => Snowflake.Parse(l)).ToArray(), userIds.ToArray());
    }
}
