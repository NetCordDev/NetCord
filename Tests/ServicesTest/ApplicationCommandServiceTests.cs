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

        await _tester.ExecuteAsync("test", [null, "xd", null], ResultHandler.DataMatch(((string?)null, "xd", (string?)null)), (string? s1 = null, string? s2 = null, string? s3 = null) => Body.Data((s1, s2, s3))).ConfigureAwait(false);

        await _tester.ExecuteAsync("test", [null, null, null, null, null, null, null, null, null], ResultHandler.DataMatch(((string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null)), (string? s1 = null, string? s2 = null, string? s3 = null, string? s4 = null, string? s5 = null, string? s6 = null, string? s7 = null, string? s8 = null, string? s9 = null) => Body.Data((s1, s2, s3, s4, s5, s6, s7, s8, s9))).ConfigureAwait(false);

        await _tester.ExecuteAsync("test", [null, null, null, null, null, null, null, null, "test"], ResultHandler.DataMatch(((string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null, "test")), (string? s1 = null, string? s2 = null, string? s3 = null, string? s4 = null, string? s5 = null, string? s6 = null, string? s7 = null, string? s8 = null, string? s9 = null) => Body.Data((s1, s2, s3, s4, s5, s6, s7, s8, s9))).ConfigureAwait(false);

        await _tester.ExecuteAsync("test", [null, null, null, null, null, null, "test", null, null], ResultHandler.DataMatch(((string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null, "test", (string?)null, (string?)null)), (string? s1 = null, string? s2 = null, string? s3 = null, string? s4 = null, string? s5 = null, string? s6 = null, string? s7 = null, string? s8 = null, string? s9 = null) => Body.Data((s1, s2, s3, s4, s5, s6, s7, s8, s9))).ConfigureAwait(false);

        await _tester.ExecuteAsync("test", [null, null, null, null, null, null, "test1", "test2", "test3"], ResultHandler.DataMatch(((string?)null, (string?)null, (string?)null, (string?)null, (string?)null, (string?)null, "test1", "test2", "test3")), (string? s1 = null, string? s2 = null, string? s3 = null, string? s4 = null, string? s5 = null, string? s6 = null, string? s7 = null, string? s8 = null, string? s9 = null) => Body.Data((s1, s2, s3, s4, s5, s6, s7, s8, s9))).ConfigureAwait(false);
    }
}
