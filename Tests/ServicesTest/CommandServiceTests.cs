namespace ServicesTest;

[TestClass]
public class CommandServiceTests : ServiceTests<CommandServiceTester>
{
    [TestMethod]
    public async ValueTask Basic()
    {
        await _tester.ExecuteMultipleAsync(
            ["test", "test-test"],
            ResultHandler.Success(),
            () => { }).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask ParameterMatching()
    {
        await _tester.ExecuteMultipleAsync(
            [
                "test test",
                "test\ntest",
                "test \ntest",
                "test \n test",
                "test           test",
                "test\n\n\n\n\n\n\n\n\ntest",
                "test\n\n  \n\n    \n\n \n\n\ntest"
            ],
            ResultHandler.DataMatch("test"),
            (string s) => Body.Data(s)).ConfigureAwait(false);
    }
}
