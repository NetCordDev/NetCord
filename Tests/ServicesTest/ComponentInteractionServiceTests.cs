namespace ServicesTest;

[TestClass]
public class ComponentInteractionServiceTests : ServiceTests<ComponentInteractionServiceTester>
{
    [TestMethod]
    public async ValueTask Basic()
    {
        await _tester.ExecuteRawAsync("test", null, ResultHandler.Success(), () => { }).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask ParameterMatching()
    {
        await _tester.ExecuteRawAsync("test", "test::xd", ResultHandler.DataMatch(((string?)null, "xd")), (string? s1 = null, string? s2 = null) => Body.Data((s1, s2))).ConfigureAwait(false);
    }
}
