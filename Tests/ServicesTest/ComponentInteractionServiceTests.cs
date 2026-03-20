namespace ServicesTest;

[TestClass]
public class ComponentInteractionServiceTests : ServiceTests<ComponentInteractionServiceTester>
{
    [TestMethod]
    public async ValueTask Basic()
    {
        await _tester.ExecuteRawAsync("test", null, ResultHandler.DataMatch("test"), () => Body.Data("test")).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask ParameterMatching()
    {
        await _tester.ExecuteRawAsync("test", "test::xd", ResultHandler.DataMatch(((string?)null, "xd")), (string? s1 = null, string? s2 = null) => Body.Data((s1, s2))).ConfigureAwait(false);

        await _tester.ExecuteRawAsync("test", "test::xd::", ResultHandler.DataMatch(((string?)null, "xd::")), (string? s1 = null, string? s2 = null) => Body.Data((s1, s2))).ConfigureAwait(false);

        await _tester.ExecuteRawAsync("test", "test::::::::", ResultHandler.DataMatch(":::::::"), (string s) => Body.Data(s)).ConfigureAwait(false);

        // Even though the last parameter is remainder, when it's empty and the parameter is optional, it will be set to the default parameter value
        await _tester.ExecuteRawAsync("test", "test:", ResultHandler.DataMatch((string?)null), (string? s = null) => Body.Data(s)).ConfigureAwait(false);

        // https://github.com/dotnet/roslyn/issues/79752
        //await _tester.ExecuteRawAsync("test", "test:", ResultHandler.DataMatch(Array.Empty<string>()), (params string[] s) => Body.DataItem(s)).ConfigureAwait(false);
    }
}
