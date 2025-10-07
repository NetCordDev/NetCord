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

    [TestMethod]
    public async ValueTask SubCommands()
    {
        var session = _tester.StartSession();

        session.AddCommandGroup("group", builder =>
        {
            var currentBuilder = builder;
            for (int i = 0; i < 100; i++)
                currentBuilder = currentBuilder.AddSubCommandGroup([$"group{i}"]);

            currentBuilder.AddSubCommand(["test1"], () => Body.Data(1));
        });

        session.AddCommandGroup("group", builder =>
        {
            var currentBuilder = builder;
            for (int i = 0; i < 100; i++)
                currentBuilder = currentBuilder.AddSubCommandGroup([$"group{i}"]);

            currentBuilder.AddSubCommand(["test2"], () => Body.Data(2));
        });

        await session.ExecuteAsync(string.Join(' ', Enumerable.Range(0, 100).Select(i => $"group{i}").Prepend("group").Append("test1")),
                                   ResultHandler.DataMatch(1)).ConfigureAwait(false);

        await session.ExecuteAsync(string.Join(' ', Enumerable.Range(0, 100).Select(i => $"group{i}").Prepend("group").Append("test2")),
                                   ResultHandler.DataMatch(2)).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask SubCommandsWithParameters()
    {
        var session = _tester.StartSession();

        session.AddCommandGroup("group", builder =>
        {
            var currentBuilder = builder;
            for (int i = 0; i < 100; i++)
                currentBuilder = currentBuilder.AddSubCommandGroup([$"group{i}"]);

            currentBuilder.AddSubCommand(["test"], (string s) => Body.Data(s));
        });

        await session.ExecuteAsync(string.Join(' ', Enumerable.Range(0, 100).Select(i => $"group{i}").Prepend("group").Append("test").Append("testvalue")),
                                   ResultHandler.DataMatch("testvalue")).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask SubCommandsPriority()
    {
        var session = _tester.StartSession();

        session.AddCommandGroup("group", builder =>
        {
            builder.AddSubCommand(["test"], (string s) => Body.Data(s));

            builder.AddSubCommandGroup(["test"], builder =>
            {
                builder.AddSubCommand(["xd"], () => Body.Data(0));
            });
        });

        await session.ExecuteAsync("group test xd", ResultHandler.DataMatch(0)).ConfigureAwait(false);

        await session.ExecuteAsync("group test test", ResultHandler.DataMatch("test")).ConfigureAwait(false);

        session.AddCommandGroup("other-group", builder =>
        {
            builder.AddSubCommand(["test"], (string s) => Body.Data(s)).WithPriority(1);

            builder.AddSubCommandGroup(["test"], builder =>
            {
                builder.AddSubCommand(["xd"], () => Body.Data(0));
            });
        });

        await session.ExecuteAsync("other-group test xd", ResultHandler.DataMatch("xd")).ConfigureAwait(false);

        await session.ExecuteAsync("other-group test test", ResultHandler.DataMatch("test")).ConfigureAwait(false);
    }
}
