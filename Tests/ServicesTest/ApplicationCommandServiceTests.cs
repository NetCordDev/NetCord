namespace ServicesTest;

[TestClass]
public class ApplicationCommandServiceTests : ServiceTests<ApplicationCommandServiceTester>
{
    [TestMethod]
    public async ValueTask Basic()
    {
        await _tester.ExecuteAsync("test", [], ResultHandler.Success(), () => { }).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask ParameterMatching()
    {
        await _tester.ExecuteAsync("test", [null, "xd"], ResultHandler.DataMatch(((string?)null, "xd")), (string? s1 = null, string? s2 = null) => Body.Data((s1, s2))).ConfigureAwait(false);
    }
}
