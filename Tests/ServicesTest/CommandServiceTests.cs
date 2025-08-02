namespace ServicesTest;

[TestClass]
public class CommandServiceTests : ServiceTests<CommandServiceTester>
{
    [TestMethod]
    public async ValueTask Basic()
    {
        var session = _tester.StartSession();

        foreach (var command in (IEnumerable<string>)["test", "test-test"])
        {
            session.AddCommand(command, () => Body.Data(command));

            await session.ExecuteAsync(command, ResultHandler.DataMatch(command)).ConfigureAwait(false);
        }
    }

    [TestMethod]
    public async ValueTask ParameterMatching()
    {
        var session = _tester.StartSession();

        session.AddCommand("test", (string s) => Body.Data(s));

        foreach (var command in (IEnumerable<string>)
            [
                "test test",
                "test\ntest",
                "test \ntest",
                "test \n test",
                "test           test",
                "test\n\n\n\n\n\n\n\n\ntest",
                "test\n\n  \n\n    \n\n \n\n\ntest"
            ])
        {
            await session.ExecuteAsync(command, ResultHandler.DataMatch("test")).ConfigureAwait(false);
        }
    }

    [TestMethod]
    public async ValueTask CommandPrioritySimple()
    {
        var session = _tester.StartSession();

        session.AddCommand("test", () => Body.Data(0));

        session.AddCommand("test", (string s = "") => Body.Data(1), 1);

        await session.ExecuteAsync(
            "test",
            ResultHandler.DataMatch(1)).ConfigureAwait(false);

        await session.ExecuteAsync(
            "test test",
            ResultHandler.DataMatch(1)).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask CommandPriorityComplex()
    {
        var session = _tester.StartSession();

        session.AddCommand("test", () => Body.Data(0));

        session.AddCommand("test", (int i = 0, string s = "") => Body.Data(1), 1);

        await session.ExecuteAsync(
            "test",
            ResultHandler.DataMatch(1)).ConfigureAwait(false);

        await session.ExecuteAsync(
            "test test",
            ResultHandler.DataMatch(1)).ConfigureAwait(false);

        await session.ExecuteAsync(
            "test 1 test",
            ResultHandler.DataMatch(1)).ConfigureAwait(false);
    }
}
