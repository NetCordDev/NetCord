using NetCord;

namespace TokenTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void BotTokenTest()
    {
        var tokenId = 803377270878109726uL;
        var rawToken = "ODAzMzc3MjcwODc4MTA5NzI2.tHis.IS.not.A.ReAl.tOkeN";

        BotToken token = new(rawToken);

        Assert.AreEqual(rawToken, token.RawToken);
        Assert.AreEqual(tokenId, token.Id);
        Assert.AreEqual($"Bot {rawToken}", token.HttpHeaderValue);

        rawToken = "ODAzMzc3MjcwOFnrMDZxSSwlq3WdlZH-3grIPBPKrSA";

        Assert.ThrowsExactly<ArgumentException>(() => new BotToken(rawToken));

        rawToken = "ODAzMzc3MjcwODc4MTA5Nz2s.tHis.IS.not.A.ReAl.tOkeN";

        Assert.ThrowsExactly<ArgumentException>(() => new BotToken(rawToken));

        Assert.ThrowsExactly<ArgumentException>(() => new BotToken(null!));
        Assert.ThrowsExactly<ArgumentException>(() => new BotToken(string.Empty));
    }

    [TestMethod]
    public void BearerTokenTest()
    {
        var rawToken = "THisiSnOtaReAltoKEN";

        BearerToken token = new(rawToken);

        Assert.AreEqual(rawToken, token.RawToken);
        Assert.AreEqual($"Bearer {rawToken}", token.HttpHeaderValue);
    }
}
