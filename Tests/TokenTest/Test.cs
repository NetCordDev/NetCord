using NetCord;

namespace TokenTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void BotTokenTest()
    {
        var tokenId = 803377270878109726uL;
        var rawToken = "ODAzMzc3MjcwODc4MTA5NzI2.GAVT0D.DDNY-77JFnrMDZxSSwlq3WdlZH-3grIPBPKrSA";

        BotToken token = new(rawToken);

        Assert.AreEqual(rawToken, token.RawToken);
        Assert.AreEqual(tokenId, token.Id);
        Assert.AreEqual($"Bot {rawToken}", token.HttpHeaderValue);

        rawToken = "ODAzMzc3MjcwOFnrMDZxSSwlq3WdlZH-3grIPBPKrSA";

        Assert.ThrowsException<ArgumentException>(() => new BotToken(rawToken));

        rawToken = "ODAzMzc3MjcwODc4MTA5Nz2s.GAVT0D.DDNY-77JFnrMDZxSSwlq3WdlZH-3grIPBPKrSA";

        Assert.ThrowsException<ArgumentException>(() => new BotToken(rawToken));

        Assert.ThrowsException<ArgumentException>(() => new BotToken(null!));
        Assert.ThrowsException<ArgumentException>(() => new BotToken(string.Empty));
    }

    [TestMethod]
    public void BearerTokenTest()
    {
        var rawToken = "PX47ggGjGyUiZVxUKi9owQ2ObzVZfg";

        BearerToken token = new(rawToken);

        Assert.AreEqual(rawToken, token.RawToken);
        Assert.AreEqual($"Bearer {rawToken}", token.HttpHeaderValue);
    }
}
